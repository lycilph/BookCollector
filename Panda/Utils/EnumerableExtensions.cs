﻿using Panda.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Panda.Utils
{
    public static class EnumerableExtensions
    {
        public static void Apply<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void DisposeAll<T>(this IEnumerable<T> source) where T : IDisposable
        {
            foreach (var item in source)
            {
                if (item is IDisposable disposable)
                    disposable.Dispose();
            }
        }

        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        public static ObservableCollectionEx<T> ToObservableCollectionEx<T>(this IEnumerable<T> source) where T : INotifyPropertyChanged
        {
            var collection = new ObservableCollectionEx<T>();
            collection.AddRange(source.ToList()); // Not entirely sure why the ToList is needed here, BUT IT IS!
            return collection;
        }
    }
}
