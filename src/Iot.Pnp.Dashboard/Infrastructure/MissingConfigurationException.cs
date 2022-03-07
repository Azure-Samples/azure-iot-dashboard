using System.Runtime.Serialization;

namespace Iot.PnpDashboard.Infrastructure
{
    [Serializable]
    internal class MissingConfigurationException : Exception
    {
        public MissingConfigurationException(string? configKey) : 
            base($"The configuration key: '{configKey}' is required but was not found")
        {
        }
    }
}