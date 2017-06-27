using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace RemoteDebugHelper
{
    internal class CopyFilesToRemote : IJob
    {
        private readonly IProgressSupport _progressSupport;
        private readonly IConfigurationReader _configurationReader;

        public CopyFilesToRemote(IProgressSupport progressSupport, IConfigurationReader configurationReader)
        {
            _progressSupport = progressSupport;
            _configurationReader = configurationReader;
        }

        public void PleaseDoTheNeedful(RunArguments runArguments)
        {
            var sourcePath = _configurationReader.GetValue(Consts.LocalWebsiteBinDirectoryConfigKey);
            var targetPath = _configurationReader.GetValue(Consts.IntermediateZipDirectoryConfigKey);
            var extsToAdd = _configurationReader.GetValue(Consts.TransferredExtensionsConfigKey).Split('|');

            var filesToAdd = Directory.GetFiles(sourcePath).Where(f => extsToAdd.Contains(Path.GetExtension(f))).ToArray();
            var zipName = $"bin_{DateTime.Now:yyyyMMdd_hhmmss}.zip";
            var zipPath = Path.Combine(targetPath, zipName);

            _progressSupport.SetupProgress(filesToAdd.Length);

            PrepareDirectory(targetPath, zipPath);

            using (var zip = new ZipFile(zipPath))
            {
                zip.SaveProgress += (obj, spe) => { if (spe.EntriesSaved > 0) _progressSupport.ChangeProgress(spe.EntriesSaved, spe.CurrentEntry.FileName); };
                zip.AddFiles(filesToAdd, string.Empty);
                zip.Save();
                Console.WriteLine("ZIP created");
            }
        }

        private static void PrepareDirectory(string targetPath, string zipPath)
        {
            if (Directory.Exists(targetPath))
            {
                var oldFilesOnTarget = Directory.GetFiles(targetPath, "bin_*.zip")
                    .Where(f => !string.Equals(f, zipPath, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (oldFilesOnTarget.Any())
                {
                    foreach (var of in oldFilesOnTarget)
                        File.Delete(of);
                }
            }
            else
            {
                Directory.CreateDirectory(targetPath);
            }
        }
    }
}