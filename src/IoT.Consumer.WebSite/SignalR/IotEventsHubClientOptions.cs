using System.ComponentModel.DataAnnotations;

namespace IoT.Consumer.WebSite.SignalR
{
    public class IotEventsHubClientOptions
    {
        [Required]
        public string ServiceEndpoint { get; set; } = null!;
    }
}
