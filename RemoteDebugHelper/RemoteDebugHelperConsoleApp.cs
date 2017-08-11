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

                //if (!container.GetInstance<ISystemUtils>().EnsureRunningAsAdmin(args))
                //    return;

                SetupJobFactory(container);

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
            var commandLineSupport = container.GetInstance<ICommandLineSupport>();
            var configuration = commandLineSupport.Setup(args);
            var job = container.GetInstance<IJobFactory>().GetJob(container, configuration.Side, configuration.Mode);

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

            container.Register<IConfiguration, Configuration.Configuration>(Lifestyle.Singleton);
            container.Register<ISystemUtils, SystemUtils>();
            container.Register<ICommandLineSupport, CommandLineSupport>();
            container.Register<IProgressSupport, ConsoleProgressSupport>();
            container.Register<IJobFactory, JobFactory>(Lifestyle.Singleton);

            container.Verify();

            return container;
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
