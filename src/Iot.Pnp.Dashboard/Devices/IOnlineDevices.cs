using Iot.PnpDashboard.Events;

namespace Iot.PnpDashboard.Devices
{
    public interface IOnlineDevices
    {
        Task<IEnumerable<string>> GetIdsAsync(string? namePattern = default, int pageSize = 100, int page = 0);
        Task<Device?> GetDeviceAsync(string id);
        Task<IEnumerable<Device>> GetAsync(string? namePattern = default, int pageSize = 100, int pageOffset = 0);
        Task<long> CountAsync();
        Task UpdateAsync(Event e);
    }
}