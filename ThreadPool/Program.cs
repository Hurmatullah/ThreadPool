using ThreadPool.IMyTasks;
using ThreadPool.Strategy;
using ThreadPool.ThreadPool;

namespace ThreadPool
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var threadPool = new ThreadPool.ThreadPool(6, WorkStrategy.WorkSharing);

            using var task = new MyTask<int>(() => 2);

            threadPool.EnqueueTask(task);

            using var continuation = task.ContinueWith(result => result + 6);

            threadPool.EnqueueTask(continuation);

            Console.WriteLine("The result is equal to: " + continuation.Result);
        }
    }
}