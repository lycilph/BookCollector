using BookCollector.Data;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace BookCollector.Screens.Series
{
    public class SeriesEntryViewModel : ItemViewModel<SeriesEntry>
    {
        public string Title { get { return (Obj.Book != null ? Obj.Book.Title : "[Unknown]"); } }
        public bool MissingInCollection { get { return Obj.MissingInCollection; } }

        public SeriesEntryViewModel(SeriesEntry obj) : base(obj)
        {
            // Add notification if book property changes to observable that needs to be disposed
            observables.Add(obj.Changed.Where(x => x.PropertyName == "Book")
                                       .Subscribe(_ => this.RaisePropertyChanged("Title")));
        }
    }
}
