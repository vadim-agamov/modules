using System;

namespace Modules.Utils
{
    public class Disposable : IDisposable
    {
        private Action _callback;
        private bool _disposed;
        
        public bool Disposed => _disposed;

        public Disposable(Action callback)
        {
            _callback = callback;
        }
        
        public void Dispose()
        {
            _disposed = true;
            _callback?.Invoke();
            _callback = null;
        }
    }
}