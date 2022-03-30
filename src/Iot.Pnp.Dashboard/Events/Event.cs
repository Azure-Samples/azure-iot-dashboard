using Azure.Messaging.EventHubs;

namespace Iot.PnpDashboard.Events
{
    public class Event
    {
        public string DeviceId { get; set; } = Guid.NewGuid().ToString();
        public string? EdgeModuleId { get; set; }
        public DateTimeOffset EnqueuedTime { get; set; } = DateTimeOffset.MinValue;
        public string? Body { get; set; } = null;
        public string? ModelId { get; set; } = null;
        public string? Operation { get; set; } = null;
        public string? MessageSource { get; set; } = null;
        public string? Component { get; set; } = null;
        public long? SequenceNumber { get; set; } = null;
        public long? Offset { get; set; } = null;
        public DateTimeOffset TelemetryProcessorOffset { get; set; } = DateTimeOffset.UtcNow;

        //public string? HubName { get; set; } = null; TODO: Interesting to have??

        public Event()
        {
        }

        public Event(EventData eventData)
        {
            if(eventData != null)
            {
                Body = eventData.EventBody.ToString();
                EnqueuedTime = eventData.EnqueuedTime;
                Offset = eventData.Offset;
                if (eventData.Properties != null)
                {
                    //eventData.Properties.TryGetValue("hubName", out var hubName);
                    //HubName = hubName?.ToString();

                    eventData.Properties.TryGetValue("opType", out var opType);
                    Operation = opType?.ToString();
                }

                if (eventData.SystemProperties != null)
                {
                    eventData.SystemProperties.TryGetValue("iothub-connection-device-id", out var deviceId);
                    DeviceId = deviceId?.ToString() ?? throw new Exception("Unable to extract deviceId from the eventData.");

                    eventData.SystemProperties.TryGetValue("iothub-connection-module-id", out var moduleId);
                    EdgeModuleId = moduleId?.ToString();

                    eventData.SystemProperties.TryGetValue("dt-dataschema", out var dataSchema);
                    ModelId = dataSchema?.ToString();

                    eventData.SystemProperties.TryGetValue("dt-subject", out var component);
                    Component = component?.ToString();

                    eventData.SystemProperties.TryGetValue("iothub-message-source", out var messageSource);
                    MessageSource = messageSource?.ToString();

                    eventData.SystemProperties.TryGetValue("x-opt-sequence-number", out var sequenceNumberStr);
                    long.TryParse(sequenceNumberStr?.ToString(), out var sequenceNumber);
                    SequenceNumber = sequenceNumber;
                }
            }    
        }
    }
}

