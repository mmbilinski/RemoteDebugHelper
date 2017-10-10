using CommandLine;
using Config.Net;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace RemoteDebugHelper.Configuration
{
    [SuppressMessage("ReSharper", "UnassignedReadonlyField")]
    public class Configuration : SettingsContainer, IConfiguration
    {
        private const string JsonConfigFileName = "RemoteDebugHelperConfig.json";

        #region Options

        private readonly Option<Side> _sideConfig = new Option<Side>(nameof(Side), Side.Dev);
        private readonly Option<Mode> _modeConfig = new Option<Mode>(nameof(Mode), Mode.Any);
        private readonly Option<string> _localWebsiteBinDirectoryConfig = new Option<string>(nameof(LocalWebsiteBinDirectory), string.Empty);
        private readonly Option<string> _remoteWebsiteDirectoryConfig = new Option<string>(nameof(RemoteWebsiteDirectory), string.Empty);
        private readonly Option<string> _intermediateZipDirectoryConfig = new Option<string>(nameof(IntermediateZipDirectory), string.Empty);
        private readonly Option<string> _transferredExtensionsConfig = new Option<string>(nameof(TransferredExtensions), string.Empty);
        private readonly Option<string> _binDirectoryNameForProductionBinariesConfig = new Option<string>(nameof(BinDirectoryNameForProductionBinaries), string.Empty);
        private readonly Option<bool> _tryRestoreDebugBinariesWhenZipNotFoundConfig = new Option<bool>(nameof(TryRestoreDebugBinariesWhenZipNotFound), false);
        private readonly Option<string> _iisAppPoolNameConfig = new Option<string>(nameof(IisAppPoolName), string.Empty);
        private readonly Option<string> _userInitialsConfig = new Option<string>(nameof(UserInitials), string.Empty);
        private readonly Option<bool> _keepRemoteBinWithInitialsConfig = new Option<bool>(nameof(KeepRemoteBinWithInitials), false);
        private readonly Option<string> _remoteDebuggerPathConfig = new Option<string>(nameof(RemoteDebuggerPath), string.Empty);
        private readonly Option<string> _remoteDebuggerParametersConfig = new Option<string>(nameof(RemoteDebuggerParameters), string.Empty);
        private readonly Option<bool> _remoteDebuggerMinimizedConfig = new Option<bool>(nameof(RemoteDebuggerMinimized), false);
        private readonly Option<int> _includeOnlyFilesModifiedInLastNDaysConfig = new Option<int>(nameof(IncludeOnlyFilesModifiedInLastNDays), -1);
        private readonly Option<bool> _autoCloseAppConfig = new Option<bool>(nameof(AutoCloseApp), false);

        #endregion

        #region Properties

        private Side? _side;
        [Option('s', "Side")]
        public Side Side
        {
            get => _side ?? _sideConfig.Value;
            set => _side = value;
        }

        private Mode? _mode;
        [Option('m', "Mode")]
        public Mode Mode
        {
            get => _mode ?? _modeConfig.Value;
            set => _mode = value;
        }

        private string _localWebsiteBinDirectory;
        [Option]
        public string LocalWebsiteBinDirectory
        {
            get => _localWebsiteBinDirectory ?? _localWebsiteBinDirectoryConfig.Value;
            set => _localWebsiteBinDirectory = value;
        }

        private string _remoteWebsiteDirectory;
        [Option]
        public string RemoteWebsiteDirectory
        {
            get => _localWebsiteBinDirectory ?? _localWebsiteBinDirectoryConfig.Value;
            set => _localWebsiteBinDirectory = value;
        }

        private string _intermediateZipDirectory;
        [Option]
        public string IntermediateZipDirectory
        {
            get => _intermediateZipDirectory ?? _intermediateZipDirectoryConfig.Value;
            set => _intermediateZipDirectory = value;
        }

        private string _transferredExtensions;
        [Option]
        public string TransferredExtensions
        {
            get => _transferredExtensions ?? _transferredExtensionsConfig.Value;
            set => _transferredExtensions = value;
        }

        private string _binDirectoryNameForProductionBinaries;
        [Option]
        public string BinDirectoryNameForProductionBinaries
        {
            get => _binDirectoryNameForProductionBinaries ?? _binDirectoryNameForProductionBinariesConfig.Value;
            set => _binDirectoryNameForProductionBinaries = value;
        }

        private bool? _tryRestoreDebugBinariesWhenZipNotFound;
        [Option]
        public bool TryRestoreDebugBinariesWhenZipNotFound
        {
            get => _tryRestoreDebugBinariesWhenZipNotFound ?? _tryRestoreDebugBinariesWhenZipNotFoundConfig.Value;
            set => _tryRestoreDebugBinariesWhenZipNotFound = value;
        }

        private string _iisAppPoolName;
        [Option]
        public string IisAppPoolName
        {
            get => _iisAppPoolName ?? _iisAppPoolNameConfig.Value;
            set => _iisAppPoolName = value;
        }

        private string _userInitials;
        [Option]
        public string UserInitials
        {
            get => _userInitials ?? _userInitialsConfig.Value;
            set => _userInitials = value;
        }

        private bool? _keepRemoteBinWithInitials;
        [Option]
        public bool KeepRemoteBinWithInitials
        {
            get => _keepRemoteBinWithInitials ?? _keepRemoteBinWithInitialsConfig.Value;
            set => _keepRemoteBinWithInitials = value;
        }

        private string _remoteDebuggerPath;
        [Option]
        public string RemoteDebuggerPath
        {
            get => _remoteDebuggerPath ?? _remoteDebuggerPathConfig.Value;
            set => _remoteDebuggerPath = value;
        }

        private string _remoteDebuggerParameters;
        [Option]
        public string RemoteDebuggerParameters
        {
            get => _remoteDebuggerParameters ?? _remoteDebuggerParametersConfig.Value;
            set => _remoteDebuggerParameters = value;
        }

        private bool? _remoteDebuggerMinimized;
        [Option]
        public bool RemoteDebuggerMinimized
        {
            get => _remoteDebuggerMinimized ?? _remoteDebuggerMinimizedConfig.Value;
            set => _remoteDebuggerMinimized = value;
        }

        private int? _includeOnlyFilesModifiedInLastNDays;
        [Option]
        public int IncludeOnlyFilesModifiedInLastNDays
        {
            get => _includeOnlyFilesModifiedInLastNDays ?? _includeOnlyFilesModifiedInLastNDaysConfig.Value;
            set => _includeOnlyFilesModifiedInLastNDays = value;
        }

        private bool? _autoCloseApp;
        [Option]
        public bool AutoCloseApp
        {
            get => _autoCloseApp ?? _autoCloseAppConfig.Value;
            set => _autoCloseApp = value;
        }

        // workaround...
        [Option]
        public bool DoNotAutoCloseApp
        {
            get => !AutoCloseApp;
            set => AutoCloseApp = !value;
        }

        #endregion

        protected override void OnConfigure(IConfigConfiguration configuration)
        {
            configuration.CacheTimeout = TimeSpan.Zero;

            // legacy configuration
            configuration.UseAppConfig();

            // general settings
            var exeConfigPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), JsonConfigFileName);
            configuration.UseJsonFile(exeConfigPath);

            // personalized settings
            var homeConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), JsonConfigFileName);
            configuration.UseJsonFile(homeConfigPath);
        }
    }
}
