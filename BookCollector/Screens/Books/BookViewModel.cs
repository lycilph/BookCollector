using BookCollector.Data;
using Panda.Infrastructure;

namespace BookCollector.Screens.Books
{
    public class BookViewModel : ItemViewModel<Book>
    {
        public string Title { get { return Obj.Title; } }
        public string Authors { get { return string.Join(",", Obj.Authors); } }
        public string ISBN { get { return Obj.ISBN; } }

        public BookViewModel(Book obj) : base(obj) { }
    }
}
