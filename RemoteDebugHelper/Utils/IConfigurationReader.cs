namespace RemoteDebugHelper
{
    internal interface IConfigurationReader
    {
        string GetValue(string key);
        bool GetBoolValue(string key);
        int? GetIntValue(string key);
    }
}