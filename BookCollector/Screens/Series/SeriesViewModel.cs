using Panda.Collections;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Series
{
    public class SeriesViewModel : ItemViewModel<Data.Series>
    {
        public string Title { get { return Obj.Title; } }
        public int EntriesCount { get { return Obj.Entries.Count; } }
        public bool Incomplete { get { return Obj.Entries.Any(e => e.MissingInCollection); } }
        public ObservableDictionary<string, string> Metadata { get { return Obj.Metadata; } }

        private ObservableCollectionEx<SeriesEntryViewModel> _Entries;
        public ObservableCollectionEx<SeriesEntryViewModel> Entries
        {
            get { return _Entries; }
            set { this.RaiseAndSetIfChanged(ref _Entries, value); }
        }

        public SeriesViewModel(Data.Series obj) : base(obj)
        {
            Entries = obj.Entries.Select(e => new SeriesEntryViewModel(e))
                                 .ToObservableCollectionEx();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                Entries.DisposeAll();
                Entries = null;
            }
        }
    }
}
