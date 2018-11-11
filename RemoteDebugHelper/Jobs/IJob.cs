using RemoteDebugHelper.Configuration;

namespace RemoteDebugHelper
{
    internal interface IJob
    {
        void PleaseDoTheNeedful();

        void ValidateConfiguration();
    }
}