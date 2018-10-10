using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;

namespace Core.Data
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

        private ReactiveList<SeriesEntry> _Entries = new ReactiveList<SeriesEntry>();
        public ReactiveList<SeriesEntry> Entries
        {
            get { return _Entries; }
            set { this.RaiseAndSetIfChanged(ref _Entries, value); }
        }

        private Dictionary<string, string> _Tags = new Dictionary<string, string>();
        public Dictionary<string, string> Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }
    }
}
