using System;
using System.IO;
using System.Linq;

namespace RemoteDebugHelper.Configuration
{
    class ConfigurationValidator
    {
        public static void ValidateNotEmptyString(string value, string propertyName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new JobException($"Empty configuration parameter: {propertyName}");
        }

        public static void ValidateDirectoryPath(string value, string propertyName)
        {
            ValidateNotEmptyString(value, propertyName);

            try
            {
                if (!new DirectoryInfo(value).Exists)
                {
                    throw new JobException($"Directory must exist in parameter {propertyName}");
                }

                if (!Directory.GetLogicalDrives().Any(ld => value.StartsWith(ld, StringComparison.OrdinalIgnoreCase)))
                {
                    throw new JobException($"Given directory must exist locally in parameter {propertyName}");
                }
            }
            catch (Exception ex) when (!(ex is JobException))
            {
                throw new JobException($"Invalid path in parameter: {propertyName}", ex);
            }
        }
    }
}
