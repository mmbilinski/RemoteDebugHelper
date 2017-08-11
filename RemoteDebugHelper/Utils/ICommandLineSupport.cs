using RemoteDebugHelper.Configuration;

namespace RemoteDebugHelper
{
    public interface ICommandLineSupport
    {
        IConfiguration Setup(string[] args);
    }
}