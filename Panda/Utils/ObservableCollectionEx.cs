using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Windows.Threading;

namespace Panda.Utils
{
    // https://stackoverflow.com/questions/269073/observablecollection-that-also-monitors-changes-on-the-elements-in-collection
    // https://github.com/kblc/Helpers/blob/master/Helpers.WPF/ObservableCollectionEx.cs
    public class ObservableCollectionEx<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        private bool suppres_notifications = false;

        public event PropertyChangedEventHandler ItemsChanged;
        public override event NotifyCollectionChangedEventHandler CollectionChanged;

        public IObservable<string> ItemsChangedEx
        {
            get
            {
                return Observable
                    .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => (sender, args) => h(args),
                        h => ItemsChanged += h,
                        h => ItemsChanged -= h)
                    .Select(x => x.PropertyName);
            }
        }

        public IObservable<string> PropertyChangedEx
        {
            get
            {
                return Observable
                    .FromEvent<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                        h => (sender, args) => h(args),
                        h => PropertyChanged += h,
                        h => PropertyChanged -= h)
                    .Select(x => x.PropertyName);
            }
        }

        public IObservable<NotifyCollectionChangedAction> CollectionChangedEx
        {
            get
            {
                return Observable
                    .FromEvent<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                        h => (sender, args) => h(args),
                        h => CollectionChanged += h,
                        h => CollectionChanged -= h)
                    .Select(x => x.Action);
            }
        }

        public IObservable<string> SomethingChanged
        {
            get
            {
                return Observable.Merge(
                    ItemsChangedEx,
                    PropertyChangedEx,
                    CollectionChangedEx.Select(x => x.ToString()));
            }
        }

        public ObservableCollectionEx() { }
        public ObservableCollectionEx(IEnumerable<T> items) : base(items) { }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            Unsubscribe(e.OldItems);
            Subscribe(e.NewItems);

            var collection_changed_handler = CollectionChanged;
            if (!suppres_notifications && collection_changed_handler != null)
            {
                using (BlockReentrancy())
                {
                    Delegate[] delegates = collection_changed_handler.GetInvocationList();
                    foreach (NotifyCollectionChangedEventHandler handler in delegates)
                    {
                        // If the subscriber is a DispatcherObject and different thread
                        if (handler.Target is DispatcherObject dispatcher_object && 
                            dispatcher_object.CheckAccess() == false)
                        {
                            // Invoke handler in the target dispatcher's thread
                            dispatcher_object.Dispatcher.Invoke(DispatcherPriority.DataBind, handler, this, e);
                        }
                        else // Execute handler as is
                            handler(this, e);
                    }
                }
            }
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (!suppres_notifications)
                base.OnPropertyChanged(e);
        }

        private void OnItemChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!suppres_notifications)
                ItemsChanged?.Invoke(sender, new PropertyChangedEventArgs(e.PropertyName));
        }

        protected override void ClearItems()
        {
            foreach (T element in this)
                element.PropertyChanged -= OnItemChanged;

            base.ClearItems();
        }

        private void Subscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                    element.PropertyChanged += OnItemChanged;
            }
        }

        private void Unsubscribe(IList iList)
        {
            if (iList != null)
            {
                foreach (T element in iList)
                    element.PropertyChanged -= OnItemChanged;
            }
        }

        public void AddRange(IEnumerable<T> items, NotifyCollectionChangedAction action = NotifyCollectionChangedAction.Reset)
        {
            if (items == null)
                throw new ArgumentNullException("items");

            CheckReentrancy();
            var starting_index = Count;
            var changed_items = new List<T>(items);

            suppres_notifications = true;
            try
            {
                foreach (var item in items)
                    Items.Add(item);
            }
            finally
            {
                suppres_notifications = false;
            }
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));

            switch (action)
            {
                case NotifyCollectionChangedAction.Add:
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, changed_items, starting_index));
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Subscribe(changed_items);
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    break;
                default:
                    throw new ArgumentException("Only Add and Reset is supported");
            }
        }
    }
}
