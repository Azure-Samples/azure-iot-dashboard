using Iot.PnpDashboard.Infrastructure;

namespace Iot.PnpDashboard.Extensions
{
    public static class ConfigurationExtensions
    {

        public static T GetValueOrDefault<T>(this IConfiguration value, string key, T defaultValue)
        {
            return value.GetValue<T>(key) ?? defaultValue;
        }

        public static T GetValueOrThrow<T>(this IConfiguration value, string key)
        {
            var result = value.GetValue<T>(key);
            if (result == null)
            {
                throw new MissingConfigurationException(key);
            }

            return result;
        }

        public static TEnum GetEnumValueOrDefault<TEnum>(this IConfiguration value, string key, TEnum defaultValue) where TEnum : struct
        {
            return Enum.TryParse<TEnum>(value.GetValue<string>(key), true, out TEnum configValue) ? configValue : defaultValue;
        }
    }
}
