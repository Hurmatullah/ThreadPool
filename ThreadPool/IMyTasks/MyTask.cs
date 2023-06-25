using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPool.IMyTasks
{
    public class MyTask<TResult> : IMyTask<TResult>
    {
        private readonly ManualResetEventSlim manualResetEvent = new ManualResetEventSlim(false, 1);

        private readonly List<Exception> exceptionsList = new List<Exception>();

        public bool IsCompleted => manualResetEvent.IsSet;

        private readonly Func<TResult> function;

        private TResult resultValue;

        public TResult Result
        {
            get
            {
                manualResetEvent.Wait();

                if (exceptionsList.Count > 0)
                {
                    throw new AggregateException(exceptionsList);
                }

                return resultValue;
            }
        }

        public MyTask(Func<TResult> func)
        {
            function = func ?? throw new ArgumentNullException(nameof(func), "Cannot create a task from a null function");
        }

        public void Process()
        {
            try
            {
                resultValue = function.Invoke();
            }
            catch (Exception e)
            {
                if (e is AggregateException exc)
                {
                    exceptionsList.AddRange(exc.InnerExceptions);
                }
                else
                {
                    exceptionsList.Add(e);
                }
            }
            finally
            {
                manualResetEvent.Set();
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            return new MyTask<TNewResult>(() => continuation(Result));
        }

        public void Dispose()
        {
            manualResetEvent?.Dispose();
        }
    }
}
