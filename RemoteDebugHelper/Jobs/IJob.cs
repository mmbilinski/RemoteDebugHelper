using RemoteDebugHelper.Configuration;

namespace RemoteDebugHelper
{
    internal interface IJob
    {
        bool RequiresAdministratorRights { get; }

        void PleaseDoTheNeedful();

        void ValidateConfiguration();
    }
}