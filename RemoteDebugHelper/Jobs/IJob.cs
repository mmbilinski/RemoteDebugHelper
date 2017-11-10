namespace RemoteDebugHelper
{
    internal interface IJob
    {
        bool RequiresAdministratorRights { get; }

        void PleaseDoTheNeedful();
    }
}