using Microsoft.AspNetCore.SignalR;

namespace Iot.PnpDashboard.EventBroadcast
{
    public class IotEventsHub : Hub
    {

        public const string HubUrl = "/deviceevents";

        public async Task Subscribe(string deviceId)
        {
            if (!String.IsNullOrEmpty(deviceId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, deviceId);
                Context.Items.Add(deviceId, null);
            }
        }

        public async Task Unsubscribe(string? deviceId)
        {
            if (deviceId is not null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, deviceId);
                Context.Items.Remove(deviceId);
            }
        }

        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"{Context.ConnectionId} connected");
            return base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? e)
        {
            await CleanUpSubscriptions();

            Console.WriteLine($"Disconnected {e?.Message} {Context.ConnectionId}");
            await base.OnDisconnectedAsync(e);
        }

        private async Task CleanUpSubscriptions()
        {
            if (Context.Items is not null)
            {
                if (Context.Items.Count > 0)
                {
                    foreach (var item in Context.Items)
                    {
                        await Unsubscribe(item.Key as string);
                    }
                }
            }
        }
    }
}
