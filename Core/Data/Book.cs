using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;

namespace Core.Data
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

        private Dictionary<string, string> _Tags = new Dictionary<string, string>();
        public Dictionary<string, string> Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }
    }
}
