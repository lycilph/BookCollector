using Newtonsoft.Json;
using Panda.Collections;
using ReactiveUI;
using System;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class SeriesEntry : ReactiveObject
    {
        private Book _Book;
        public Book Book
        {
            get { return _Book; }
            set { this.RaiseAndSetIfChanged(ref _Book, value); }
        }

        private int _Position;
        public int Position
        {
            get { return _Position; }
            set { this.RaiseAndSetIfChanged(ref _Position, value); }
        }

        // This is used to check for incomplete series
        private DateTime _LastChecked;
        public DateTime LastChecked
        {
            get { return _LastChecked; }
            set { this.RaiseAndSetIfChanged(ref _LastChecked, value); }
        }

        private bool _MissingInCollection = true;
        public bool MissingInCollection
        {
            get { return _MissingInCollection; }
            set { this.RaiseAndSetIfChanged(ref _MissingInCollection, value); }
        }

        private ObservableDictionary<string, string> _Metadata = new ObservableDictionary<string, string>();
        public ObservableDictionary<string, string> Metadata
        {
            get { return _Metadata; }
            set { this.RaiseAndSetIfChanged(ref _Metadata, value); }
        }
    }
}
