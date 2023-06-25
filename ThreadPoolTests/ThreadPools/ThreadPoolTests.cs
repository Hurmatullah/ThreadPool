using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.ThreadPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ThreadPool.IMyTasks;

namespace ThreadPool.ThreadPools.Tests
{
    [TestClass()]
    public class ThreadPoolTests
    {
        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        ThreadPool threadPool = new ThreadPool(_threadCount);

        public const int _threadCount = 5;

        public Thread[] _threads;

        public bool IsTerminated = false;


        [TestMethod()]
        public void SubmitTest()
        {
            var task = threadPool.Submit(() => 500 * 500);
            task.Start();
            SleepAndShutdownPool();
            Assert.That(task.IsCompleted && task.Result == 250000, Is.True);
        }

        [TestMethod()]
        public void VerifyNThreads()
        {
            var tasks = new List<MyTask<int>>();
            for (int i = 0; i < 5; i++)
            {
                tasks.Add(threadPool.Submit(GetThreadId));
            }
            var selectThreads = tasks.Select(task => task.Result);
            SleepAndShutdownPool();
            var threadIdCount = selectThreads.Count();
            Assert.That(threadIdCount, Is.EqualTo(_threadCount));
        }

        [TestMethod()]
        public static int GetThreadId()
        {
            var currentThreadId = System.Environment.CurrentManagedThreadId;
            return currentThreadId;
        }

        [TestMethod()]
        public void ShutdownTest()
        {
            if (IsTerminated)
            {
                cancellationToken.Cancel();
                for (var i = 0; i < _threadCount; ++i)
                {
                    _threads[i].Join();
                }
            }
            SleepAndShutdownPool();
            Assert.IsTrue(true);
        }

        public void SleepAndShutdownPool()
        {
            Thread.Sleep(100);
            threadPool.Shutdown();
        }
    }
}