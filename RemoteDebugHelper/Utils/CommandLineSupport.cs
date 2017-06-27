using Fclp;
using System;

namespace RemoteDebugHelper
{
    class CommandLineSupport : ICommandLineSupport
    {
        public RunArguments Setup(string[] args)
        {
            var argsParser = new FluentCommandLineParser<RunArguments>();

            argsParser.Setup(a => a.Side)
                .As('s', "side")
                .Required()
                .WithDescription("On which environment are you running?");
            argsParser.Setup(a => a.Mode)
                .As('m', "mode")
                .SetDefault(Mode.Any)
                .WithDescription("Are you starting or finishing?");

            var parseResult = argsParser.Parse(args);
            if (parseResult.HasErrors)
                throw new ArgumentException($"Invalid args: {parseResult.ErrorText}");

            return argsParser.Object;
        }
    }
}