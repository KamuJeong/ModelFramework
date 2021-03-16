using System;
using System.Threading;
using System.Threading.Tasks;

namespace Kamu.ModelFramework
{
    public abstract class ModelProviderWithExclusiveScheduler : ModelProvider
    {
        private ConcurrentExclusiveSchedulerPair _schedulerPair = new ConcurrentExclusiveSchedulerPair();

        protected override ValueTask InvokeAsync(Action action, TaskScheduler scheduler)
        {
            return new ValueTask(Task.Factory.StartNew(action, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                _schedulerPair.ExclusiveScheduler));
        }

        protected override ValueTask<T> InvokeAsync<T>(Func<T> function, TaskScheduler scheduler)
        {
            return new ValueTask<T>(Task.Factory.StartNew(function, CancellationToken.None, TaskCreationOptions.DenyChildAttach,
                                                                _schedulerPair.ExclusiveScheduler));
        }       
    }
}
