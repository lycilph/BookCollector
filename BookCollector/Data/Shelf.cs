using Newtonsoft.Json;
using ReactiveUI;
using System.Collections.Generic;
using System.Diagnostics;

namespace BookCollector.Data
{
    [DebuggerDisplay("Name = {Name}, Books {Books.Count}")]
    [JsonObject(MemberSerialization.OptOut)]
    public class Shelf : ReactiveObject
    {
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { this.RaiseAndSetIfChanged(ref _Name, value); }
        }

        private List<Book> _Books = new List<Book>();
        [JsonIgnore] // Have to ignore this, since it causes circular references in the Json
        public List<Book> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public void Add(Book book)
        {
            if (!Books.Contains(book))
                Books.Add(book);
        }

        public void Remove(Book book)
        {
            Books.Remove(book);
        }
    }
}
