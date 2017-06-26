using Ionic.Zip;
using System;
using System.IO;
using System.Linq;

namespace RemoteDebugHelper
{
    internal class CopyFilesToRemote : IJob
    {
        public void PleaseDoTheNeedful(RunArguments runArguments)
        {
            var sourcePath = @"c:\inetpub\wwwroot\NextQiagen\Website\bin\";
            var targetPath = Path.Combine(@"u:\", "remoteDebug");

            var extsToAdd = new[] { ".dll", ".pdb" };
            var filesToAdd = Directory.GetFiles(sourcePath).Where(f => extsToAdd.Contains(Path.GetExtension(f)));

            var zipName = $"bin_{DateTime.Now:yyyyMMdd_hhmmss}.zip";
            var zipPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "bugfixing", zipName);

            using (var zip = new ZipFile(zipPath))
            {
                //zip.SaveProgress += (obj, spe) => { if (spe.EntriesSaved > 0) Util.Progress = (int)(spe.EntriesSaved * 100 / spe.EntriesTotal); };
                zip.AddFiles(filesToAdd, string.Empty);
                zip.Save();
                Console.WriteLine("ZIP created");
            }

            if (Directory.Exists(targetPath))
            {
                var oldFilesOnTarget = Directory.GetFiles(targetPath, "bin_*.zip")
                    .Where(f => !string.Equals(f, zipPath, StringComparison.OrdinalIgnoreCase));

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