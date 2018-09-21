using Newtonsoft.Json;
using ReactiveUI;
using System;
using System.Diagnostics;

namespace Core.Data
{
    [DebuggerDisplay("Name = {Name}, Filename = {Filename}, Id = {Id}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Collection : ReactiveObject
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _Filename;
        [JsonIgnore]
        public string Filename
        {
            get { return _Filename; }
            set { this.RaiseAndSetIfChanged(ref _Filename, value); }
        }

        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private ReactiveList<Book> _Books = new ReactiveList<Book>();
        public ReactiveList<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private ReactiveList<Note> _Notes = new ReactiveList<Note>();
        public ReactiveList<Note> Notes
        {
            get { return _Notes; }
            set { this.RaiseAndSetIfChanged(ref _Notes, value); }
        }
    }
}
