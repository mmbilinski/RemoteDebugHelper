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
    }
}