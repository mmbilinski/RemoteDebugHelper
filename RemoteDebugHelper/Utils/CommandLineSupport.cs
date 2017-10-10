using RemoteDebugHelper.Configuration;
using SimpleInjector;
using System;

namespace RemoteDebugHelper
{
    class CommandLineSupport : ICommandLineSupport
    {
        private readonly Container _container;

        public CommandLineSupport(Container container)
        {
            _container = container;
        }

        public IConfiguration Setup(string[] args)
        {
            throw new NotSupportedException("Not supported for now");
        }
    }
}