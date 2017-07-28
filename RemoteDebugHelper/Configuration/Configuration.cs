using Config.Net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace RemoteDebugHelper.Configuration
{
    [SuppressMessage("ReSharper", "UnassignedReadonlyField")]
    public class Configuration : SettingsContainer, IConfiguration
    {
        #region Options

        private readonly Option<string> _localWebsiteBinDirectory = new Option<string>(nameof(LocalWebsiteBinDirectory), string.Empty);
        private readonly Option<string> _remoteWebsiteDirectory = new Option<string>(nameof(RemoteWebsiteDirectory), string.Empty);
        private readonly Option<string> _intermediateZipDirectory = new Option<string>(nameof(IntermediateZipDirectory), string.Empty);
        private readonly Option<string> _transferredExtensions = new Option<string>(nameof(TransferredExtensions), string.Empty);
        private readonly Option<string> _binDirectoryNameForProductionBinaries = new Option<string>(nameof(BinDirectoryNameForProductionBinaries), string.Empty);
        private readonly Option<bool> _tryRestoreDebugBinariesWhenZipNotFound = new Option<bool>(nameof(TryRestoreDebugBinariesWhenZipNotFound), false);
        private readonly Option<string> _iisAppPoolName = new Option<string>(nameof(IisAppPoolName), string.Empty);
        private readonly Option<string> _userInitials = new Option<string>(nameof(UserInitials), string.Empty);
        private readonly Option<bool> _keepRemoteBinWithInitials = new Option<bool>(nameof(KeepRemoteBinWithInitials), false);
        private readonly Option<string> _remoteDebuggerPath = new Option<string>(nameof(RemoteDebuggerPath), string.Empty);
        private readonly Option<string> _remoteDebuggerParameters = new Option<string>(nameof(RemoteDebuggerParameters), string.Empty);
        private readonly Option<bool> _remoteDebuggerMinimized = new Option<bool>(nameof(RemoteDebuggerMinimized), false);
        private readonly Option<int> _includeOnlyFilesModifiedInLastNDays = new Option<int>(nameof(LocalWebsiteBinDirectory), -1);
        private readonly Option<bool> _autoCloseApp = new Option<bool>(nameof(AutoCloseApp), false);

        #endregion

        #region Properties

        public string LocalWebsiteBinDirectory => _localWebsiteBinDirectory.Value;
        public string RemoteWebsiteDirectory => _remoteWebsiteDirectory.Value;
        public string IntermediateZipDirectory => _intermediateZipDirectory.Value;
        public string TransferredExtensions => _transferredExtensions.Value;
        public string BinDirectoryNameForProductionBinaries => _binDirectoryNameForProductionBinaries.Value;
        public bool TryRestoreDebugBinariesWhenZipNotFound => _tryRestoreDebugBinariesWhenZipNotFound.Value;
        public string IisAppPoolName => _iisAppPoolName.Value;
        public string UserInitials => _userInitials.Value;
        public bool KeepRemoteBinWithInitials => _keepRemoteBinWithInitials.Value;
        public string RemoteDebuggerPath => _remoteDebuggerPath.Value;
        public string RemoteDebuggerParameters => _remoteDebuggerParameters.Value;
        public bool RemoteDebuggerMinimized => _remoteDebuggerMinimized.Value;
        public int IncludeOnlyFilesModifiedInLastNDays => _includeOnlyFilesModifiedInLastNDays.Value;
        public bool AutoCloseApp => _autoCloseApp.Value;

        #endregion

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.CacheTimeout = TimeSpan.Zero;

            // legacy configuration
            configuration.UseAppConfig();

            // local settings
            var homeConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "RemoteDebugHelperConfig.json");
            configuration.UseJsonFile(homeConfigPath);

            // personalized settings
            configuration.UseJsonFile("RemoteDebugHelperConfig.json");

            // command line
            // TODO: replace current solution with Config.Net
            // configuration.UseCommandLineArgs();
        }
    }
}
