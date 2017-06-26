using SimpleInjector;
using System;

namespace RemoteDebugHelper
{
    class RemoteDebugHelperConsoleApp
    {
        static void Main(string[] args)
        {
            Container container = ConfigureContainer();

            var commandLineSupport = container.GetInstance<ICommandLineSupport>();
            var jobFactory = container.GetInstance<IJobFactory>();

            jobFactory.RegisterJob<CopyFilesToRemote>(Side.Dev, Mode.Any);
            jobFactory.RegisterJob<CopyIntoWebsiteBin>(Side.Env, Mode.Start);

            try
            {
                var runArguments = commandLineSupport.Setup(args);
                var job = jobFactory.GetJob(runArguments.Side, runArguments.Mode);

                job.PleaseDoTheNeedful(runArguments);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            Console.ReadKey(true);
        }

        private static Container ConfigureContainer()
        {
            Container container = new Container();
            container.Register<ICommandLineSupport>();
            container.Register<IConfigurationReader>();
            container.Register<IJobFactory>();
            container.Verify();
            return container;
        }
    }
}
