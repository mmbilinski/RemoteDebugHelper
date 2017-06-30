using Microsoft.Web.Administration;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;

namespace RemoteDebugHelper
{
    class SystemUtils : ISystemUtils
    {
        private readonly IConfigurationReader _configurationReader;

        public SystemUtils(IConfigurationReader configurationReader)
        {
            _configurationReader = configurationReader;
        }

        public bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public bool EnsureRunningAsAdmin(string[] args)
        {
            if (!IsAdministrator())
            {
                StartProcessAsAdmin(Assembly.GetExecutingAssembly().Location, args);

                return false;
            }

            return true;
        }

        public bool StartRemoteDebugger()
        {
            var rdPath = _configurationReader.GetValue(Consts.ConfigKeys.RemoteDebuggerPath);

            if (!string.IsNullOrWhiteSpace(rdPath))
            {
                var rdArgs = _configurationReader.GetValue(Consts.ConfigKeys.RemoteDebuggerParameters);
                StartProcessAsAdmin(rdPath, new[] { rdArgs });
            }

            return true;
        }

        public bool CloseRemoteDebugger(bool politely)
        {
            var result = true;
            var rdPath = _configurationReader.GetValue(Consts.ConfigKeys.RemoteDebuggerPath);

            if (!string.IsNullOrWhiteSpace(rdPath))
            {
                var processName = Path.GetFileNameWithoutExtension(rdPath);
                var processes = Process.GetProcessesByName(processName);

                foreach (var process in processes)
                {
                    using (process)
                    {
                        if (politely)
                            result &= process.CloseMainWindow();
                        else
                        {
                            process.Kill();
                        }
                    }
                }
            }

            return result;
        }

        public void RecycleAppPool()
        {
            var appPool = GetAppPool();

            appPool.Recycle();
        }

        public void RestartAppPool()
        {
            var appPool = GetAppPool();

            appPool.Stop();

            int appPoolStopPatienceCounter = 20;

            while (appPool.State == ObjectState.Stopping)
            {
                if (appPoolStopPatienceCounter == 0)
                {
                    throw new InvalidOperationException("Unable to stop AppPool, please do the needful and kill w3wp process");
                }

                Thread.Sleep(500);
            }

            appPool.Start();
        }

        private ApplicationPool GetAppPool()
        {
            var sm = new ServerManager();
            var appPool = sm.ApplicationPools.SingleOrDefault(ap => string.Equals(ap.Name,
                _configurationReader.GetValue(Consts.ConfigKeys.IisAppPoolName), StringComparison.OrdinalIgnoreCase));

            return appPool ?? throw new InvalidOperationException("App pool not found");
        }

        private bool StartProcessAsAdmin(string path, string[] args)
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo(path)
                {
                    Verb = "runas",
                    Arguments = string.Join(" ", args),
                    UseShellExecute = false
                }
            };

            try
            {
                return p.Start();
            }
            catch
            {
                return false;
            }
        }
    }
}