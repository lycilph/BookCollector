using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;

namespace Core.Data
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

        private bool _MissingInCollection = true;
        public bool MissingInCollection
        {
            get { return _MissingInCollection; }
            set { this.RaiseAndSetIfChanged(ref _MissingInCollection, value); }
        }

        private Dictionary<string, string> _Tags = new Dictionary<string, string>();
        public Dictionary<string, string> Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }
    }
}
