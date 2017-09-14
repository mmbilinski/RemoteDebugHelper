using Ionic.Zip;
using RemoteDebugHelper.Configuration;
using System;
using System.IO;
using System.Linq;

namespace RemoteDebugHelper
{
    internal class CopyFilesToRemote : IJob
    {
        private readonly IProgressSupport _progressSupport;
        private readonly IConfiguration _configuration;

        public CopyFilesToRemote(IProgressSupport progressSupport, IConfiguration configuration)
        {
            _progressSupport = progressSupport;
            _configuration = configuration;
        }

        public void PleaseDoTheNeedful()
        {
            Console.WriteLine(_configuration.IncludeOnlyFilesModifiedInLastNDays >= 0
                ? $"Taking changes from last {_configuration.IncludeOnlyFilesModifiedInLastNDays} days."
                : "Taking all files without date filtering.");

            var targetPath = _configuration.IntermediateZipDirectory;
            var filesToAdd = GetFilesToTransfer();
            var zipName = $"bin_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
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

        private string[] GetFilesToTransfer()
        {
            var sourcePath = _configuration.LocalWebsiteBinDirectory;
            var extsToAdd = _configuration.TransferredExtensions.Split('|');
            var filesToTransfer = new DirectoryInfo(sourcePath).GetFiles().Where(f => extsToAdd.Contains(f.Extension));
            var includeFilesFromLastNDays = _configuration.IncludeOnlyFilesModifiedInLastNDays;

            if (includeFilesFromLastNDays >= 0)
            {
                filesToTransfer = filesToTransfer
                    .Where(f => f.LastWriteTime > DateTime.Today.AddDays(-includeFilesFromLastNDays));
            }

            return filesToTransfer.Select(f => f.FullName).ToArray();
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