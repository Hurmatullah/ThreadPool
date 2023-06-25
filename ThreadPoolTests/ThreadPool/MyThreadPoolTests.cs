using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.ThreadPool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThreadPool.Strategy;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ThreadPool.IMyTasks;
using static System.Net.WebRequestMethods;

namespace ThreadPool.ThreadPool.Tests
{
    [TestClass()]
    public class MyThreadPoolTests
    {
        private const int MaxThreads = 10;

        private WorkStrategy workStrategy;

        [TestMethod()]
        public void MyThreadPoolTest()
        {
            var threadPool = new ThreadPool(MaxThreads, workStrategy);
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void EnqueueTaskTest()
        {
            var task = new MyTask<int>(() => 2 + 6);
            Assert.IsFalse(false);
        }

        [TestMethod()]
        public void EnqueuTaskTest2()
        {
            var threadPool = new ThreadPool(6, WorkStrategy.WorkSharing);
            var task = new MyTask<int>(() => 2 + 6);
            threadPool.EnqueueTask(task);
            Assert.That(task.Result, Is.EqualTo(8));
        }

        [TestMethod()]
        public void DisposeTest()
        {
            var threadPool = new ThreadPool(MaxThreads, workStrategy);
            threadPool.Dispose();
            Assert.Throws<ObjectDisposedException>(() => threadPool.Dispose());
        }   
    }
}