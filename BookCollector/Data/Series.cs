using Newtonsoft.Json;
using Panda.Collections;
using ReactiveUI;
using System;

namespace BookCollector.Data
{
    [JsonObject(MemberSerialization.OptOut)]
    public class Series : ReactiveObject
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        // This is used to check for incomplete series
        private DateTime _LastChecked;
        public DateTime LastChecked
        {
            get { return _LastChecked; }
            set { this.RaiseAndSetIfChanged(ref _LastChecked, value); }
        }

        private ObservableCollectionEx<SeriesEntry> _Entries = new ObservableCollectionEx<SeriesEntry>();
        public ObservableCollectionEx<SeriesEntry> Entries
        {
            get { return _Entries; }
            set { this.RaiseAndSetIfChanged(ref _Entries, value); }
        }

        private ObservableDictionary<string, string> _Metadata = new ObservableDictionary<string, string>();
        public ObservableDictionary<string, string> Metadata
        {
            get { return _Metadata; }
            set { this.RaiseAndSetIfChanged(ref _Metadata, value); }
        }
    }
}
