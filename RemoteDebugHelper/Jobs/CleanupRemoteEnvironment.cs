using RemoteDebugHelper.Configuration;
using System;
using System.IO;

namespace RemoteDebugHelper
{
    internal class CleanupRemoteEnvironment : IJob
    {
        private readonly IProgressSupport _progressSupport;
        private readonly IConfiguration _configuration;
        private readonly ISystemUtils _systemUtils;

        public CleanupRemoteEnvironment(IProgressSupport progressSupport, IConfiguration configuration,
            ISystemUtils systemUtils)
        {
            _progressSupport = progressSupport;
            _configuration = configuration;
            _systemUtils = systemUtils;
        }

        public bool RequiresAdministratorRights => true;

        public void PleaseDoTheNeedful()
        {
            var websitePath = _configuration.RemoteWebsiteDirectory;
            var userInitials = _configuration.UserInitials;
            var binInitialsPath = Path.Combine(websitePath, $"bin{userInitials}");
            var binProdFolderName = _configuration.BinDirectoryNameForProductionBinaries;
            var binProdPath = Path.Combine(websitePath, binProdFolderName);
            var binPath = Path.Combine(websitePath, "bin");

            // if bin<initials> is already there, delete or rename it
            if (Directory.Exists(binInitialsPath))
            {
                if (_configuration.KeepRemoteBinWithInitials)
                {
                    Directory.Move(binInitialsPath,
                    Path.Combine(websitePath, $"{binInitialsPath}_{DateTime.Now:yyyyMMdd_hhmmss}"));
                }
                else
                {
                    Directory.Delete(binInitialsPath, true);
                }
            }

            // ensure existance of production binaries
            if (!Directory.Exists(binProdPath))
            {
                throw new InvalidOperationException("Cannot find a backup folder with production binaries.");
            }

            // delete development binaries
            Directory.Move(binPath, binInitialsPath);

            // move binProd to bin
            Directory.Move(binProdPath, binPath);

            // refresh AppPool
            _systemUtils.RecycleAppPool();

            // close remote debugger
            _systemUtils.CloseRemoteDebugger(true);
        }
    }
}