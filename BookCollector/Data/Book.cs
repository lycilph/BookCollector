using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;

namespace BookCollector.Data
{
    [DebuggerDisplay("Title = {Title}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Book : ReactiveObject
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private List<string> _Authors = new List<string>();
        public List<string> Authors
        {
            get { return _Authors; }
            set { this.RaiseAndSetIfChanged(ref _Authors, value); }
        }

        private string _ISBN;
        public string ISBN
        {
            get { return _ISBN; }
            set { this.RaiseAndSetIfChanged(ref _ISBN, value); }
        }

        private string _ISBN13;
        public string ISBN13
        {
            get { return _ISBN13; }
            set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        }

        private string _Description;
        public string Description
        {
            get { return _Description; }
            set { this.RaiseAndSetIfChanged(ref _Description, value); }
        }

        private Shelf _Shelf;
        public Shelf Shelf
        {
            get { return _Shelf; }
            set { this.RaiseAndSetIfChanged(ref _Shelf, value); }
        }

        private HashSet<Tag> _Tags = new HashSet<Tag>();
        public HashSet<Tag> Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }

        private Dictionary<string, string> _Metadata = new Dictionary<string, string>();
        public Dictionary<string, string> Metadata
        {
            get { return _Metadata; }
            set { this.RaiseAndSetIfChanged(ref _Metadata, value); }
        }
    }
}
