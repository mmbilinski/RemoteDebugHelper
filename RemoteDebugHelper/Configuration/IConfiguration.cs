namespace RemoteDebugHelper.Configuration
{
    public interface IConfiguration
    {
        string LocalWebsiteBinDirectory { get; }
        string RemoteWebsiteDirectory { get; }
        string IntermediateZipDirectory { get; }
        string TransferredExtensions { get; }
        string BinDirectoryNameForProductionBinaries { get; }
        bool TryRestoreDebugBinariesWhenZipNotFound { get; }
        string IisAppPoolName { get; }
        string UserInitials { get; }
        bool KeepRemoteBinWithInitials { get; }
        string RemoteDebuggerPath { get; }
        string RemoteDebuggerParameters { get; }
        bool RemoteDebuggerMinimized { get; }
        int IncludeOnlyFilesModifiedInLastNDays { get; }
        bool AutoCloseApp { get; }
    }
}