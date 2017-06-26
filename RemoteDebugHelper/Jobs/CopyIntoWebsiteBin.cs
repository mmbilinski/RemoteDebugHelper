using Ionic.Zip;
using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace RemoteDebugHelper
{
    internal class CopyIntoWebsiteBin : IJob
    {
        public void PleaseDoTheNeedful(RunArguments runArguments)
        {
            string websitePath = @"c:\inetpub\wwwroot\NextQiagen\Website\bin\";
            string zipPath = @""; // TODO

            string binJenkinsFolderName = "binJenkins";
            string binJenkinsPath = Path.Combine(websitePath, binJenkinsFolderName);
            string binPath = Path.Combine(websitePath, "bin");

            // if binJenkins is already there, rename it
            if (Directory.Exists(binJenkinsPath))
                Directory.Move(binJenkinsPath, Path.Combine(websitePath, $"{binJenkinsFolderName}_{DateTime.Now:yyyyMMdd_hhmmss}"));

            // move bin to binJenkins
            Directory.Move(binPath, binJenkinsPath);

            // copy binJenkins to bin
            CreateDirectoryCopy(binJenkinsPath, binPath);

            // unpack zip to bin
            using (var zip = new ZipFile(zipPath))
            {
                zip.ExtractAll(binPath, ExtractExistingFileAction.OverwriteSilently);
            }

            // restart AppPool
            RestartAppPool();
        }

        void CreateDirectoryCopy(string sourcePath, string targetPath)
        {
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));

            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
        }

        void RestartAppPool()
        {
            var sm = new ServerManager();
            var appPool = sm.ApplicationPools.SingleOrDefault(ap => string.Equals(ap.Name, "NextQiagenAppPool", StringComparison.OrdinalIgnoreCase));
            if (appPool == null)
                throw new InvalidOperationException("App pool not found");
            appPool.Stop();

            int appPoolStopPatienceCounter = 20;

            while (appPool.State == ObjectState.Stopping)
            {
                if (appPoolStopPatienceCounter == 0)
                {
                    Console.WriteLine("Unable to stop AppPool, please do the needful and kill w3wp");
                    return;
                }

                Thread.Sleep(500);
            }

            appPool.Start();
        }
    }
}