using ReactiveUI;
using System.Diagnostics;

namespace BookCollector.Goodreads.Data
{
    [DebuggerDisplay("Title = {Title}")]
    public class GoodreadsCSVBook : ReactiveObject
    {
        public string BookId { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string AdditionalAuthors { get; set; }
        public string ISBN { get; set; }
        public string ISBN13 { get; set; }
        public string Bookshelves { get; set; }
        public string ExclusiveShelf { get; set; }
        public string Source { get { return "Goodreads CSV"; } }

        private bool _IsDuplicate;
        public bool IsDuplicate
        {
            get { return _IsDuplicate; }
            set { this.RaiseAndSetIfChanged(ref _IsDuplicate, value); }
        }
    }
}
