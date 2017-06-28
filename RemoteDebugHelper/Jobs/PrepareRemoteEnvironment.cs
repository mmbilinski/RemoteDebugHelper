using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace RemoteDebugHelper
{
    internal class PrepareRemoteEnvironment : IJob
    {
        private readonly IProgressSupport _progressSupport;
        private readonly IConfigurationReader _configurationReader;
        private readonly ISystemUtils _systemUtils;

        public PrepareRemoteEnvironment(IProgressSupport progressSupport, IConfigurationReader configurationReader,
            ISystemUtils systemUtils)
        {
            _progressSupport = progressSupport;
            _configurationReader = configurationReader;
            _systemUtils = systemUtils;
        }

        public void PleaseDoTheNeedful(RunArguments runArguments)
        {
            var sourcePath = _configurationReader.GetValue(Consts.ConfigKeys.IntermediateZipDirectory);
            var zipName = new DirectoryInfo(sourcePath).GetFiles("bin_*.zip")
                .OrderByDescending(fi => fi.CreationTime)
                .FirstOrDefault()
                ?.Name;
            string zipPath;

            if (string.IsNullOrWhiteSpace(zipName))
            {
                if (_configurationReader.GetBoolValue(Consts.ConfigKeys.TryRestoreDebugBinariesWhenZipNotFound))
                {
                    // TODO
                    throw new NotImplementedException("This is not implemented for now.");
                }
                else
                {
                    throw new InvalidOperationException("ZIP with debug binaries not found. Consider setting TryRestoreDebugBinariesWhenZipNotFound flag to True.");
                }
            }
            else
            {
                zipPath = Path.Combine(sourcePath, zipName);
            }

            var websitePath = _configurationReader.GetValue(Consts.ConfigKeys.RemoteWebsiteDirectory);
            var binProdFolderName =  _configurationReader.GetValue(Consts.ConfigKeys.BinDirectoryNameForProductionBinaries);
            var binProdPath = Path.Combine(websitePath, binProdFolderName);
            var binPath = Path.Combine(websitePath, "bin");

            // if binProd is already there, rename it
            if (Directory.Exists(binProdPath))
                Directory.Move(binProdPath, Path.Combine(websitePath, $"{binProdFolderName}_{DateTime.Now:yyyyMMdd_hhmmss}"));

            // move bin to binProd
            Directory.Move(binPath, binProdPath);

            // copy binProd to bin
            CreateDirectoryCopy(binProdPath, binPath);

            // unpack zip to bin
            using (var zip = new ZipFile(zipPath))
            {
                Console.WriteLine($"Starting unzip to {binPath}");
                _progressSupport.SetupProgress(zip.Count);
                zip.ExtractProgress += (obj, epe) => { if (epe.EntriesExtracted > 0) _progressSupport.ChangeProgress(epe.EntriesExtracted, epe.CurrentEntry.FileName); };
                zip.ExtractAll(binPath, ExtractExistingFileAction.OverwriteSilently);
                Console.WriteLine("Finished unzip");
            }

            // refresh AppPool
            _systemUtils.RecycleAppPool();

            // start remote debugger
            if (!_systemUtils.StartRemoteDebugger())
            {
                throw new InvalidOperationException("Starting remote debugger failed.");
            }
        }

        private void CreateDirectoryCopy(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }
    }
}