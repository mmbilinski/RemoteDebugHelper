using CommandLine;
using RemoteDebugHelper.Configuration;
using SimpleInjector;
using System;
using System.Reflection;

namespace RemoteDebugHelper
{
    class RemoteDebugHelperConsoleApp
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Remote debug helper v{Assembly.GetExecutingAssembly().GetName().Version} started");

            try
            {
                var container = ConfigureContainer();

                if (!SetupConfiguration(container, args))
                {
                    return;
                }

                SetupJobFactory(container);

                container.Verify();

                var app = new RemoteDebugHelperConsoleApp();
                app.RunApplication(container, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        
        private void RunApplication(Container container, string[] args)
        {
            var configuration = container.GetInstance<IConfiguration>();
            var job = container.GetInstance<IJobFactory>().GetJob(container, configuration.Side, configuration.Mode);

            if (job.RequiresAdministratorRights && !container.GetInstance<ISystemUtils>().EnsureRunningAsAdmin(args))
                return;

            Console.WriteLine($"You are on {configuration.Side} and I'll try to do {configuration.Mode} action");

            job.PleaseDoTheNeedful();

            Console.Write("Done.");

            if (!configuration.AutoCloseApp)
            {
                Console.ReadKey(true);
            }
        }

        private static Container ConfigureContainer()
        {
            var container = new Container();

            container.Register<ISystemUtils, SystemUtils>();
            container.Register<ICommandLineSupport, CommandLineSupport>();
            container.Register<IProgressSupport, ConsoleProgressSupport>();
            container.Register<IJobFactory, JobFactory>(Lifestyle.Singleton);

            return container;
        }

        private static bool SetupConfiguration(Container container, string[] args)
        {
            var parser = new Parser(s =>
            {
                s.CaseSensitive = false;
                s.CaseInsensitiveEnumValues = true;
                s.HelpWriter = Console.Error;
            });

            IConfiguration configuration = parser.ParseArguments<Configuration.Configuration>(args)
                .MapResult(
                    config => config,
                    errors => null);// throw new ArgumentException($"Invalid configuration:\n{string.Join(Environment.NewLine, errors)}"));

            if (configuration != null)
            {
                container.Register(() => configuration, Lifestyle.Singleton);

                return true;
            }

            return false;
        }

        private static void SetupJobFactory(Container container)
        {
            var jobFactory = container.GetInstance<IJobFactory>();

            jobFactory.RegisterJob<CopyFilesToRemote>(Side.Dev, Mode.Any);
            jobFactory.RegisterJob<PrepareRemoteEnvironment>(Side.Env, Mode.Start);
            jobFactory.RegisterJob<CleanupRemoteEnvironment>(Side.Env, Mode.Finish);
            jobFactory.RegisterJob<InteractiveRemoteEnvironment>(Side.Env, Mode.Interactive);
        }
    }
}
