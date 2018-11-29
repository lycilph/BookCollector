using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Panda.Collections
{
    // Stolen from here: http://blogs.microsoft.co.il/shimmy/2010/12/26/observabledictionarylttkey-tvaluegt-c/
    public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private const string count_string = "Count";
        private const string indexer_string = "Item[]";
        private const string keys_string = "Keys";
        private const string values_string = "Values";

        protected IDictionary<TKey, TValue> InternalDictionary { get; private set; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableDictionary()
        {
            InternalDictionary = new Dictionary<TKey, TValue>();
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
        {
            InternalDictionary = new Dictionary<TKey, TValue>(dictionary);
        }
        public ObservableDictionary(IEqualityComparer<TKey> comparer)
        {
            InternalDictionary = new Dictionary<TKey, TValue>(comparer);
        }
        public ObservableDictionary(int capacity)
        {
            InternalDictionary = new Dictionary<TKey, TValue>(capacity);
        }
        public ObservableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
        {
            InternalDictionary = new Dictionary<TKey, TValue>(dictionary, comparer);
        }
        public ObservableDictionary(int capacity, IEqualityComparer<TKey> comparer)
        {
            InternalDictionary = new Dictionary<TKey, TValue>(capacity, comparer);
        }

        public ICollection<TKey> Keys
        {
            get { return InternalDictionary.Keys; }
        }

        public ICollection<TValue> Values
        {
            get { return InternalDictionary.Values; }
        }

        public int Count
        {
            get { return InternalDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return InternalDictionary.IsReadOnly; }
        }

        public TValue this[TKey key]
        {
            get { return InternalDictionary[key]; }
            set { Insert(key, value, false); }
        }

        public void Add(TKey key, TValue value)
        {
            Insert(key, value, true);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Insert(item.Key, item.Value, true);
        }

        public void AddRange(IDictionary<TKey, TValue> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            if (items.Count > 0)
            {
                if (InternalDictionary.Count > 0)
                {
                    if (items.Keys.Any((k) => InternalDictionary.ContainsKey(k)))
                        throw new ArgumentException("An item with the same key has already been added.");
                    else
                        foreach (var item in items) InternalDictionary.Add(item);
                }
                else
                    InternalDictionary = new Dictionary<TKey, TValue>(items);

                OnPropertyChanged();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items.ToArray()));
            }
        }

        public bool Remove(TKey key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var removed = InternalDictionary.Remove(key);
            if (removed)
            {
                OnPropertyChanged();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }

            return removed;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public bool ContainsKey(TKey key)
        {
            return InternalDictionary.ContainsKey(key);
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return InternalDictionary.Contains(item);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return InternalDictionary.TryGetValue(key, out value);
        }

        public void Clear()
        {
            if (InternalDictionary.Count > 0)
            {
                InternalDictionary.Clear();

                OnPropertyChanged();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            InternalDictionary.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return InternalDictionary.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)InternalDictionary).GetEnumerator();
        }

        private void Insert(TKey key, TValue value, bool add)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            if (InternalDictionary.TryGetValue(key, out TValue item))
            {
                if (add) throw new ArgumentException("An item with the same key has already been added.");
                if (Equals(item, value)) return;
                InternalDictionary[key] = value;

                var action = NotifyCollectionChangedAction.Replace;
                var new_item = new KeyValuePair<TKey, TValue>(key, value);
                var old_item = new KeyValuePair<TKey, TValue>(key, item);

                OnPropertyChanged();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, new_item, old_item));
            }
            else
            {
                InternalDictionary[key] = value;

                var action = NotifyCollectionChangedAction.Add;
                var new_item = new KeyValuePair<TKey, TValue>(key, value);

                OnPropertyChanged();
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, new_item));
            }
        }

        private void OnPropertyChanged()
        {
            OnPropertyChanged(count_string);
            OnPropertyChanged(indexer_string);
            OnPropertyChanged(keys_string);
            OnPropertyChanged(values_string);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
