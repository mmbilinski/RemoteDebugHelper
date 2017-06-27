using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;

namespace RemoteDebugHelper
{
    class AdminUtils : IAdminUtils
    {
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
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().Location)
                    {
                        Verb = "runas",
                        Arguments = string.Join(" ", args),
                        UseShellExecute = false
                    }
                };

                p.Start();
                return false;
            }

            return true;
        }
    }
}