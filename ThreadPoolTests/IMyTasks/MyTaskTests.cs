using Microsoft.VisualStudio.TestTools.UnitTesting;
using ThreadPool.IMyTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Assert = NUnit.Framework.Assert;
using ThreadPool.Strategy;
using static System.Net.WebRequestMethods;
using System.Runtime.CompilerServices;

namespace ThreadPool.IMyTasks.Tests
{
    [TestClass()]
    public class MyTaskTests
    {
        private const int MaxThread = 10;

        private WorkStrategy workStrategy;

        [TestMethod()]
        public void MyTaskTest()
        {
            var myTask = new MyTask<int>(() => 2 + 7);
            Assert.IsFalse(false);
        }

        [TestMethod()]
        public void RunMethodTest()
        {
            var myTask = new MyTask<int>(() => 2 + 7);
            myTask.Process();
            Assert.IsTrue(true);
        }

        [TestMethod()]
        public void MultipleContinuationTest()
        {
            using var threadPool = new ThreadPool.ThreadPool(MaxThread, workStrategy);
            using var task1 = new MyTask<int>(() => 4);
            using var task2 = task1.ContinueWith(result => result * 8);
            using var task3 = task2.ContinueWith(result => result * 10);
            threadPool.EnqueueTask(task1);
            threadPool.EnqueueTask(task2);
            threadPool.EnqueueTask(task3);
            Assert.Multiple(() =>
            {
                Assert.That(task2.Result, Is.EqualTo(32));
                Assert.That(task3.Result, Is.EqualTo(320));
            });
        }
    }
}