namespace RemoteDebugHelper
{
    internal interface ISystemUtils
    {
        bool IsAdministrator();
        bool EnsureRunningAsAdmin(string[] args);
        bool StartRemoteDebugger();
        bool CloseRemoteDebugger(bool politely);
        void RecycleAppPool();
        void RestartAppPool();
    }
}