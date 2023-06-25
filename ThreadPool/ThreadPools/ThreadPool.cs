using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ThreadPool.IMyTasks;
using ThreadPool.Strategy;

namespace ThreadPool.ThreadPool
{
    public class ThreadPool : IDisposable
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        private readonly object disposeLockObject = new object();

        private readonly Random randomGenerator = new Random();

        private readonly Strategy.Strategy[] workerThreads;

        private readonly WorkStrategy workStrategy;

        private bool isDisposed;

        public ThreadPool(int threadPoolSize, WorkStrategy strategy)
        {
            workStrategy = strategy;
            workerThreads = new Strategy.Strategy[threadPoolSize];

            for (var i = 0; i < threadPoolSize; i++)
            {
                workerThreads[i] = new Strategy.Strategy(this, i, cancellationTokenSource.Token);
            }

            foreach (var workerThread in workerThreads)
            {
                workerThread.StartThreads();
            }
        }

        public void EnqueueTask<TResult>(IMyTask<TResult> task)
        {
            if (task == null)
            {
                throw new ArgumentNullException(nameof(task), "The task argument must be provided and cannot be null");
            }

            lock (disposeLockObject)
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException("Cannot add task to the disposed thread pool.");
                }

                var workerId = workerThreads.OrderBy(workerThread => workerThread.TasksCount).First().Id;

                workerThreads[workerId].AddTaskToQueue(task.Process);
            }
        }

        internal bool StealTasks(out Action? task, int workerId)
        {
            var workerThreadLength = workerThreads.Length;
            var markWorkerId = (workerId + randomGenerator.Next(workerThreadLength - 1)) % workerThreadLength;
            return workerThreads[markWorkerId].StealTasks(out task);
        }

        internal void ShareTasks(int workerId)
        {
            var size = workerThreads[workerId].TasksCount;

            if (randomGenerator.Next(size + 1) == size)
            {
                if (workerThreads.Length != 1)
                {
                    var vId = (workerId + randomGenerator.Next(1, workerThreads.Length)) % workerThreads.Length;
                    var (minimum, maximum) = workerId <= vId ? (workerId, vId) : (vId, workerId);
                    lock (workerThreads[minimum])
                    {
                        lock (workerThreads[maximum])
                        {
                            workerThreads[minimum].BalanceQueue(workerThreads[maximum]);

                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (disposeLockObject)
            {
                if (isDisposed)
                {
                    throw new ObjectDisposedException("Cannot add task to the disposed thread pool.");
                }

                isDisposed = true;
                cancellationTokenSource.Cancel();

                foreach (var workerThread in workerThreads)
                {
                    workerThread.Join();
                }

                cancellationTokenSource.Dispose();
            }
        }
    }
}
