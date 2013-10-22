using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DefendUranus.Helpers
{
    delegate Task CancellableAsyncOperation(CancellationToken cancellation);

    class AsyncOperation
    {
        #region Attributes
        readonly CancellableAsyncOperation _operation;
        CancellationTokenSource _cancellation;
        Task _task;
        #endregion

        public AsyncOperation(CancellableAsyncOperation operation)
        {
            _operation = operation;
        }

        public bool IsActive
        {
            get { return _task != null && !_task.IsCompleted; }
            set
            {
                if(value)
                {
                    if (!IsActive)
                    {
                        _cancellation = new CancellationTokenSource();
                        _task = _operation(_cancellation.Token);
                    }
                }
                else
                {
                    if (_cancellation != null)
                    {
                        _cancellation.Cancel();
                        _cancellation = null;
                    }
                }
            }
        }
    }
}
