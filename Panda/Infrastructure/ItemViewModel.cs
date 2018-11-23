using ReactiveUI;
using System;
using System.Reactive.Disposables;

namespace Panda.Infrastructure
{
    public class ItemViewModel<T> : ReactiveObject, IDisposable
    {
        private bool disposed = false; // To detect redundant calls
        private CompositeDisposable observables;

        public T Obj { get; protected set; }

        public ItemViewModel(T obj)
        {
            Obj = obj;

            if (Obj is ReactiveObject ro)
            {
                observables = new CompositeDisposable(
                    ro.Changing.Subscribe(x => this.RaisePropertyChanging(x.PropertyName)),
                    ro.Changed.Subscribe(x => this.RaisePropertyChanged(x.PropertyName)));
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing && observables != null)
            {
                observables.Dispose();
                observables = null;
            }

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
