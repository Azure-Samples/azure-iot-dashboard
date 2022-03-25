using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Iot.PnpDashboard.Configuration;
using Iot.PnpDashboard.Devices;
using Iot.PnpDashboard.EventBroadcast;
using Iot.PnpDashboard.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Sockets;

namespace Iot.PnpDashboard.Events
{
    public class EventHubProcessorService : BackgroundService
    {
        struct PartitionTrackingData
        {
            public int EventCount;
            public DateTime LastCheckpoint;
            public ProcessEventArgs PreviousEventArgs;
        }

        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
        private readonly TimeSpan _maxWaitTime = TimeSpan.FromSeconds(30);

        private readonly AppConfiguration _configuration;
        private readonly ConnectionMultiplexer _cacheConnection;
        private readonly IDatabaseAsync _onlineDevicesCache;
        private readonly ISubscriber _onlineDevicesSubscriber;
        private readonly EventProcessorClientFactory _processorFactory;
        private readonly IHubContext<IotEventsHub> _signalR;
        private readonly CancellationTokenSource _cancellationSource;
        private readonly IHostApplicationLifetime _lifetime;
        private readonly ILogger _logger;

        private EventProcessorClient? _eventProcessorClient;

        private int _consecutiveErrorsHandlingEvents = 0;
        private DateTime _lastEventReceivedTimeStamp = DateTime.MinValue;
        private ConcurrentDictionary<string, PartitionTrackingData> _partitionTracking = new ConcurrentDictionary<string, PartitionTrackingData>();

        public EventHubProcessorService(AppConfiguration configuration,
            RedisConnectionFactory redisConnectionFactory,
            IHubContext<IotEventsHub> signalr, 
            IHostApplicationLifetime lifetime, 
            ILogger<EventHubProcessorService> logger)
        {
            if (signalr is null) throw new ArgumentNullException(nameof(signalr));

            _configuration = configuration;
            var _cacheConnectionFactory = new RedisConnectionFactory(configuration);
            _cacheConnection = redisConnectionFactory.GetConnection();
            _onlineDevicesSubscriber = _cacheConnection.GetSubscriber();
            _onlineDevicesCache = _cacheConnection.GetDatabase();
            _processorFactory = new EventProcessorClientFactory(_configuration);
            _signalR = signalr;
            _lifetime = lifetime;
            _logger = logger;
            _cancellationSource = new CancellationTokenSource();
    }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await WaitForAppStartup(_lifetime, stoppingToken))
            {
                return;
            }

            _lifetime.ApplicationStopping.Register(async () =>
            {
                await StopEventProcessorAsync();

            });

            _eventProcessorClient = await CreateEventProcessorAsync();
            await _eventProcessorClient.StartProcessingAsync(_cancellationSource.Token);
        }

        private async Task ProcessEventAsync(ProcessEventArgs args)
        {
            try
            {
                if (args.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                if (args.HasEvent)
                {
                    _lastEventReceivedTimeStamp = DateTime.UtcNow;
                    Event iotEvent = new Event(args.Data);

                    if (iotEvent.DeviceId is not null)
                    {
                        iotEvent.TelemetryProcessorOffset = DateTimeOffset.UtcNow;
                        //Send the event to the device group
                        await _signalR.Clients.Groups(iotEvent.DeviceId).SendAsync("DeviceEvent", iotEvent).ConfigureAwait(false);

                        //send the event to the online devices channel in redis
                        await PublishOnlineDeviceEventAsync(iotEvent).ConfigureAwait(false);
                    }
                }
                else
                {
                    if (DateTime.UtcNow - _lastEventReceivedTimeStamp > _maxWaitTime)
                    {
                        _logger.LogInformation($"EventHubProcessorService is alive and waiting for events. Last event received at {_lastEventReceivedTimeStamp.ToString()}.");
                    }
                }

                await UpdateCheckpointAsync(args).ConfigureAwait(false);

                _consecutiveErrorsHandlingEvents = 0; //We succeedded processing the event, reset the consecutive errors counter.
            }
            catch(Exception ex)
            {
                _consecutiveErrorsHandlingEvents++;
                if(_consecutiveErrorsHandlingEvents < 100)
                {
                    _logger.LogWarning(ex, "EventHubProcessorService exception handling new event.");
                }
                else
                {
                    _logger.LogError(ex, $"EventHubProcessorService exception. The maximum number of consecutive errors handling events has been reached. Stopping the processing.");
                    _cancellationSource.Cancel();
                }
            }
            return;
        }
        private async Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            try
            {
                if (args.CancellationToken.IsCancellationRequested)
                {
                    return;
                }

                // If out of memory, signal for cancellation. Consider other non-recoverable exceptions. Make sense to handle this exception? (it came from the EH processor client samples)
                if (args.Exception is OutOfMemoryException)
                {
                    _logger.LogError(args.Exception, $"The system is running out of memory. Unable to recover. The event processing is being cancelled.");
                    _cancellationSource.Cancel();
                    return;
                }

                //Network Connection Lost -> Consider not recoverable retry from UI, directly cancel
                if(args.Exception is SocketException)
                {
                    _logger.LogError(args.Exception, $"There is a network error during the processing of Operation '{args.Operation}' in PartitionId '{args.PartitionId}'. Unable to recover. The event processing is being cancelled.");
                    _cancellationSource.Cancel();
                    return;
                }

                //TODO: Advance scenario -> transient error or failure due to failover. Investigate further and review code.
                if ((args.Exception is EventHubsException && ((EventHubsException)args.Exception).IsTransient) ||
                    (args.Exception is EventHubsException && ((EventHubsException)args.Exception).Message.Contains("Namespace cannot be resolved")))

                {
                    await RestartEventProcessorAsync(args);
                }
                else if (_eventProcessorClient is not null && !_eventProcessorClient.IsRunning && !_cancellationSource.IsCancellationRequested)
                {
                    //if the processor is not running, we re-start the process by stopping and starting again.
                    _logger.LogWarning(args.Exception, $"Error processing Operation '{args.Operation}' in PartitionId '{args.PartitionId}. The processor is not running and there is no cancellation request. Trying to recover by re-starting the processing.");
                    await _eventProcessorClient.StopProcessingAsync();
                    await _eventProcessorClient.StartProcessingAsync(_cancellationSource.Token);
                }
                else
                {
                    //TODO: review processing exists to ensure resilency
                    _logger.LogError(args.Exception, $"Unrecoverable error processing Operation '{args.Operation}' in PartitionId '{args.PartitionId}'.  The event processing is being cancelled.");
                    _cancellationSource.Cancel();
                }
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected exception handling proccesor error. Unable to recover. The event processing is being cancelled.");
                _cancellationSource.Cancel();
            }
        }

        private Task PartitionInitializingAsync(PartitionInitializingEventArgs args)
        {
            try
            {
                if (args.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                // If no checkpoint was found, start processing events enqueued now or in the future.
                EventPosition startPositionWhenNoCheckpoint =
                    EventPosition.FromEnqueuedTime(DateTimeOffset.UtcNow);
                args.DefaultStartingPosition = startPositionWhenNoCheckpoint;

                _logger.LogInformation($"Partition with Id={ args.PartitionId } is initializing. Starting processing new events from { startPositionWhenNoCheckpoint }.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in ProcessorErrorHandler. Cancelling Processing");
                _cancellationSource.Cancel();
            }

            return Task.CompletedTask;
        }

        private Task PartitionClosingAsync(PartitionClosingEventArgs args)
        {
            try
            {
                if (args.CancellationToken.IsCancellationRequested)
                {
                    return Task.CompletedTask;
                }

                string description = args.Reason switch
                {
                    ProcessingStoppedReason.OwnershipLost =>
                        "Another processor claimed ownership",

                    ProcessingStoppedReason.Shutdown =>
                        "The processor is shutting down",

                    _ => args.Reason.ToString()
                };
                _logger.LogWarning($"Partition with Id={ args.PartitionId } is closing. Reason: { description }.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Unexpected exception handling partition closing event for Partition with Id={ args.PartitionId } is closing.");
            }

            return Task.CompletedTask;
        }

        private async Task<bool> WaitForAppStartup(IHostApplicationLifetime lifetime, CancellationToken stoppingToken)
        {
            var startedSource = new TaskCompletionSource();
            var cancelledSource = new TaskCompletionSource();

            using var reg1 = lifetime.ApplicationStarted.Register(() => startedSource.SetResult());
            using var reg2 = stoppingToken.Register(() => cancelledSource.SetResult());

            Task completedTask = await Task.WhenAny(
                startedSource.Task,
                cancelledSource.Task).ConfigureAwait(false);

            // If the completed tasks was the "app started" task, return true, otherwise false
            return completedTask == startedSource.Task;
        }

        private async Task<EventProcessorClient> CreateEventProcessorAsync()
        {
            //TODO: Include some logging information
            var eventProcessor = await _processorFactory.CreateAsync();

            eventProcessor.PartitionInitializingAsync += PartitionInitializingAsync;
            eventProcessor.PartitionClosingAsync += PartitionClosingAsync;
            eventProcessor.ProcessEventAsync += ProcessEventAsync;
            eventProcessor.ProcessErrorAsync += ProcessErrorAsync;

            return eventProcessor;
        }

        private async Task StopEventProcessorAsync()
        {
            _cancellationSource.Cancel(); //Is that correct?
            if (_eventProcessorClient is not null)
            {
                await _eventProcessorClient.StopProcessingAsync(_cancellationSource.Token);
                _eventProcessorClient.PartitionInitializingAsync -= PartitionInitializingAsync;
                _eventProcessorClient.PartitionClosingAsync -= PartitionClosingAsync;
                _eventProcessorClient.ProcessEventAsync -= ProcessEventAsync;
                _eventProcessorClient.ProcessErrorAsync -= ProcessErrorAsync;
            }
        }

        private async Task RestartEventProcessorAsync(ProcessErrorEventArgs args)
        {
            await semaphoreSlim.WaitAsync(); //Ensure only one process is going to execute the reconnection
            try
            {
                if (!_cancellationSource.IsCancellationRequested)
                {
                    await StopEventProcessorAsync();

                    _logger.LogWarning(args.Exception, $"Transient error for Operation '{args.Operation}' in PartitionId '{args.PartitionId}'. Trying to reconnect...");

                    _eventProcessorClient = await CreateEventProcessorAsync();
                    await _eventProcessorClient.StartProcessingAsync(_cancellationSource.Token);
                }
                else
                {
                    //TODO: flag reconnection to ensure we are already in the process of reconnection?
                    _logger.LogWarning(args.Exception, $"Transient error for Operation '{args.Operation}' in PartitionId '{args.PartitionId}'. Reconnecting process already initiated.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected exception trying to recover from a transient error");
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        private async Task UpdateCheckpointAsync(ProcessEventArgs args)
        {
            string partition = args.Partition.PartitionId;
            PartitionTrackingData tracking = _partitionTracking.AddOrUpdate(
                key: partition, 
                addValue: new PartitionTrackingData () { EventCount = 1, LastCheckpoint= DateTime.UtcNow, PreviousEventArgs = default }, 
                updateValueFactory: (id, trackingData) => {
                    trackingData.EventCount++;
                    if (args.HasEvent)
                    {
                        trackingData.PreviousEventArgs = args;
                    }
                return trackingData;
            });

            //Checkpoint every 50 events or 1 minute processed
            if (args.HasEvent && (tracking.EventCount >= 50 || DateTime.UtcNow > tracking.LastCheckpoint.AddMinutes(1)))
            {
                await args.UpdateCheckpointAsync(_cancellationSource.Token);
                _partitionTracking[partition] = new PartitionTrackingData() { EventCount = 0, LastCheckpoint = DateTime.UtcNow, PreviousEventArgs = default };
            }
            else if(!args.HasEvent && tracking.PreviousEventArgs.HasEvent)
            {
                await tracking.PreviousEventArgs.UpdateCheckpointAsync(_cancellationSource.Token);
                _partitionTracking[partition] = new PartitionTrackingData() { EventCount = 0, LastCheckpoint = DateTime.UtcNow, PreviousEventArgs = default };
            }
        }

        private async Task PublishOnlineDeviceEventAsync(Event iotEvent)
        {
            await _onlineDevicesSubscriber.PublishAsync(
                OnlineDevicesService.OnlineDevicesUpdatesChannel,
                JsonConvert.SerializeObject(iotEvent), CommandFlags.FireAndForget).ConfigureAwait(false);

        }
    }
}
