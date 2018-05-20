using System.Threading.Tasks;
using System.Windows.Threading;

namespace CosmosQueryEditor.Infrastructure
{
    public class DispatcherTaskSchedulerProvider
    {
        public static DispatcherTaskSchedulerProvider Create(Dispatcher dispatcher)
        {
            TaskScheduler taskScheduler = null;
            dispatcher.Invoke(() => taskScheduler = TaskScheduler.FromCurrentSynchronizationContext());
            return new DispatcherTaskSchedulerProvider(taskScheduler);
        }

        private DispatcherTaskSchedulerProvider(TaskScheduler taskScheduler)
        {
            TaskScheduler = taskScheduler;
        }

        public TaskScheduler TaskScheduler { get; }
    }
}