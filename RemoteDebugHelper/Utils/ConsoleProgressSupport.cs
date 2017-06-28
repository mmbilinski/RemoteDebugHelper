using Konsole;
using System;

namespace RemoteDebugHelper
{
    internal class ConsoleProgressSupport : IProgressSupport
    {
        private ProgressBar _progressBar;

        public void SetupProgress(int max)
        {
            _progressBar = new ProgressBar(max);
        }

        public void SetupProgress(int max, string description)
        {
            Console.WriteLine(description);
            SetupProgress(max);
        }

        public void ChangeProgress(int current)
        {
            if (_progressBar == null)
                throw new InvalidOperationException("Progress bar not set up");

            ChangeProgress(current, String.Empty);
        }

        public void ChangeProgress(int current, string description)
        {
            if (_progressBar == null)
                throw new InvalidOperationException("Progress bar not set up");

            _progressBar.Refresh(current, description);
        }
    }
}