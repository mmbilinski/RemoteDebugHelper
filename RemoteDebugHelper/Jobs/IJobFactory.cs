using SimpleInjector;

namespace RemoteDebugHelper
{
    internal interface IJobFactory
    {
        void RegisterJob<T>(Side side, Mode mode) where T : IJob;
        IJob GetJob(Container container, Side side, Mode mode);
    }
}