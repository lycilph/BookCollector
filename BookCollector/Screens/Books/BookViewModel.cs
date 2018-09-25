using Core.Data;
using Panda.Infrastructure;
using System.Collections.Generic;

namespace BookCollector.Screens.Books
{
    public class BookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(",", Obj.Authors); } }
        public string ISBN { get { return Obj.ISBN; } }
        public string ISBN13 { get { return Obj.ISBN13; } }
        public Dictionary<string, string> Tags { get { return Obj.Tags; } }

        public BookViewModel(Book obj) : base(obj) { }
    }
}
