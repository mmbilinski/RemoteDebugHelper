using SimpleInjector;
using System;
using System.Collections.Generic;

namespace RemoteDebugHelper
{
    internal class JobFactory : IJobFactory
    {
        private readonly Dictionary<Tuple<Side, Mode>, Type> _jobs = new Dictionary<Tuple<Side, Mode>, Type>();

        public void RegisterJob<T>(Side side, Mode mode) where T : IJob
        {
            _jobs[Tuple.Create(side, mode)] = typeof(T);
        }

        public IJob GetJob(Container container, Side side, Mode mode)
        {
            if (side == Side.Dev && mode != Mode.Any)
                mode = Mode.Any;

            if (_jobs.TryGetValue(Tuple.Create(side, mode), out Type jobType))
            {
                var job = container.GetInstance(jobType) as IJob;

                if (job == null)
                    throw new InvalidOperationException("Job misconfigured.");

                return job;
            }

            throw new ArgumentException("Unknown job for this arguments.");
        }
    }
}