using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.Devices
{
    public interface IOnlineDevices
    {
        Task<IEnumerable<Device>> GetAsync(string? namePattern = default, int pageSize = 100, int pageOffset = 0);
        Task<long> CountAsync();
        Task UpdateAsync(Event e);
    }
}