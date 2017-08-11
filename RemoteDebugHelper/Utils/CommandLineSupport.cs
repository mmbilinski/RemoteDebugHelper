using CommandLine;
using CommandLine.Text;
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
            var configuration = _container.GetInstance<Configuration.Configuration>();

            var parser = new Parser(s => 
            {
                s.CaseSensitive = false;
                s.CaseInsensitiveEnumValues = true;
            });
            var parserResult = parser.ParseArguments(() => configuration, args);

            if (parserResult.Tag == ParserResultType.Parsed)
                return configuration;

            throw new ArgumentException(HelpText.AutoBuild(parserResult));
        }
    }
}