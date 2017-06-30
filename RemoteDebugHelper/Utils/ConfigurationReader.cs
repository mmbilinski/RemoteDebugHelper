using System.Configuration;
using System.Linq;

namespace RemoteDebugHelper
{
    class ConfigurationReader : IConfigurationReader
    {
        public string GetValue(string key)
        {
            if (ConfigurationManager.AppSettings.AllKeys.Contains(key))
                return ConfigurationManager.AppSettings[key];

            throw new SettingsPropertyNotFoundException();
        }

        public bool GetBoolValue(string key)
        {
            return bool.TryParse(GetValue(key), out bool result) && result;
        }

        public int? GetIntValue(string key)
        {
            return int.TryParse(GetValue(key), out int result) ? result : (int?)null;
        }
    }
}