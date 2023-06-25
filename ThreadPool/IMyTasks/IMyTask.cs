using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThreadPool.IMyTasks
{
    public interface IMyTask<out TResult> : IDisposable
    {
        bool IsCompleted { get; }

        TResult Result { get; }

        void Process();

        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation);
    }
}
