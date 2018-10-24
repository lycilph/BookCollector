using System;

namespace Panda.Utils
{
    public abstract class DisposableBase : IDisposable
    {
        private bool disposed = false; // To detect redundant calls

        protected virtual void DisposeInternal() {}

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
                DisposeInternal();

            disposed = true;
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
    }
}
