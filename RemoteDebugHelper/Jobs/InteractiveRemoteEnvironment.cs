using SimpleInjector;
using System;
using System.Threading;

namespace RemoteDebugHelper
{
    internal class InteractiveRemoteEnvironment : IJob
    {
        private readonly IJobFactory _jobFactory;
        private readonly Container _container;
        private readonly ManualResetEvent _stopAppEvent;

        public InteractiveRemoteEnvironment(Container container, IJobFactory jobFactory)
        {
            _jobFactory = jobFactory;
            _container = container;

            _stopAppEvent = new ManualResetEvent(false);
        }

        public void PleaseDoTheNeedful(RunArguments runArguments)
        {
            Console.CancelKeyPress += SetupCancelKeystroke;

            StartDebuggingSession(runArguments);

            _stopAppEvent.WaitOne();

            FinishDebuggingSession(runArguments);

            Console.CancelKeyPress -= SetupCancelKeystroke;
        }

        private void StartDebuggingSession(RunArguments runArguments)
        {
            var jobStart = _jobFactory.GetJob(_container, Side.Env, Mode.Start);
            jobStart.PleaseDoTheNeedful(runArguments);
            Console.WriteLine("Debugging session started. Press Ctrl+C to finish.");
        }

        private void FinishDebuggingSession(RunArguments runArguments)
        {
            var jobFinish = _jobFactory.GetJob(_container, Side.Env, Mode.Finish);
            jobFinish.PleaseDoTheNeedful(runArguments);
            Console.WriteLine("Debugging session finished.");
        }

        private void SetupCancelKeystroke(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _stopAppEvent.Set();
        }
    }
}
