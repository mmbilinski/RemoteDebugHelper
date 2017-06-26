namespace RemoteDebugHelper
{
    internal interface IJobFactory
    {
        void RegisterJob<T>(Side side, Mode mode) where T : IJob;
        IJob GetJob(Side side, Mode mode);
    }
}