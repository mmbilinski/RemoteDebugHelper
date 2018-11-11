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

        public bool RequiresAdministratorRights => true;

        public void PleaseDoTheNeedful()
        {
            Console.CancelKeyPress += SetupCancelKeystroke;

            StartDebuggingSession();

            _stopAppEvent.WaitOne();

            FinishDebuggingSession();

            Console.CancelKeyPress -= SetupCancelKeystroke;
        }

        public void ValidateConfiguration()
        {
            throw new NotImplementedException();
        }

        private void StartDebuggingSession()
        {
            var jobStart = _jobFactory.GetJob(_container, Side.Env, Mode.Start);
            jobStart.PleaseDoTheNeedful();
            Console.WriteLine("Debugging session started. Press Ctrl+C to finish.");
        }

        private void FinishDebuggingSession()
        {
            var jobFinish = _jobFactory.GetJob(_container, Side.Env, Mode.Finish);
            jobFinish.PleaseDoTheNeedful();
            Console.WriteLine("Debugging session finished.");
        }

        private void SetupCancelKeystroke(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            _stopAppEvent.Set();
        }
    }
}
