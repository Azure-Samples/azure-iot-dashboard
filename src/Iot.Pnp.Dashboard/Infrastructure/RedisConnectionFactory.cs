using Iot.PnpDashboard.Configuration;
using StackExchange.Redis;
using System.Diagnostics.Contracts;

namespace Iot.PnpDashboard.Infrastructure
{
    public class RedisConnectionFactory
    {
        private static Lazy<ConnectionMultiplexer> _connectionMultiplexer;
        private readonly AppConfiguration _configuration;
        public RedisConnectionFactory(AppConfiguration configuration)
        {
            _configuration = configuration;
            _connectionMultiplexer = CreateConnection();
        }

        private Lazy<ConnectionMultiplexer> CreateConnection()
        {
            return new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(_configuration.RedisConnStr);
            });
        }

        public ConnectionMultiplexer GetConnection()
        {
            return _connectionMultiplexer.Value;
        }
    }
}
