using Fclp;
using Ionic.Zip;
using Microsoft.Web.Administration;
using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace RemoteDebugHelper
{
    class RemoteDebugHelperConsoleApp
    {
        static void Main(string[] args)
        {
            var argsParser = new FluentCommandLineParser<RunArguments>();

            argsParser.Setup(a => a.Side)
                .As('s', "side")
                .Required()
                .WithDescription("On which environment are you running?");
            argsParser.Setup(a => a.Mode)
                .As('m', "mode")
                .Required()
                .WithDescription("Are you starting or finishing?");

            var parseResult = argsParser.Parse(args);
            if (!parseResult.HasErrors)
            {
                var app = new RemoteDebugHelperConsoleApp();
                app.DoTheNeedful(argsParser.Object);
            }
            else
            {
                Console.WriteLine(parseResult.ErrorText);
            }

            Console.ReadKey(true);
        }

        private void DoTheNeedful(RunArguments runArguments)
        {
            if (runArguments.Side == Side.Dev)
                DoDevJob();

            Console.WriteLine($"s = {runArguments.Side}, m = {runArguments.Mode}");
        }

        private void DoDevJob()
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

        void DoEnvJob(string zipPath, string websitePath)
        {
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

    class RunArguments
    {
        public Side Side { get; set; }
        public Mode Mode { get; set; }
    }

    enum Side
    {
        Env,
        Dev
    }

    enum Mode
    {
        Start,
        Finish
    }
}
