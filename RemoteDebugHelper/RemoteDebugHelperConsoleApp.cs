using SimpleInjector;
using System;

namespace RemoteDebugHelper
{
    class RemoteDebugHelperConsoleApp
    {
        static void Main(string[] args)
        {
            var container = ConfigureContainer();

            if (!container.GetInstance<IAdminUtils>().EnsureRunningAsAdmin(args))
                return;

            SetupJobFactory(container);

            try
            {
                var app = new RemoteDebugHelperConsoleApp();
                app.RunApplication(container, args);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadKey(true);
        }

        private void RunApplication(Container container, string[] args)
        {
            var commandLineSupport = container.GetInstance<ICommandLineSupport>();
            var runArguments = commandLineSupport.Setup(args);
            var job = container.GetInstance<IJobFactory>().GetJob(container, runArguments.Side, runArguments.Mode);

            job.PleaseDoTheNeedful(runArguments);
        }

        private static Container ConfigureContainer()
        {
            var container = new Container();

            container.Register<IAdminUtils, AdminUtils>();
            container.Register<ICommandLineSupport, CommandLineSupport>();
            container.Register<IProgressSupport, ConsoleProgressSupport>();
            container.Register<IConfigurationReader, ConfigurationReader>();
            container.Register<IJobFactory, JobFactory>(Lifestyle.Singleton);

            container.Verify();

            return container;
        }

        private static void SetupJobFactory(Container container)
        {
            var jobFactory = container.GetInstance<IJobFactory>();

            jobFactory.RegisterJob<CopyFilesToRemote>(Side.Dev, Mode.Any);
            jobFactory.RegisterJob<CopyIntoWebsiteBin>(Side.Env, Mode.Start);
        }
    }
}
