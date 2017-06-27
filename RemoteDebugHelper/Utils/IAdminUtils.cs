namespace RemoteDebugHelper
{
    internal interface IAdminUtils
    {
        bool IsAdministrator();
        bool EnsureRunningAsAdmin(string[] args);
    }
}