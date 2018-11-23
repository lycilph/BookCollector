using BookCollector.Application;
using BookCollector.Data;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BookDetailsViewModel : ScreenBase
    {
        private IStateManager state_manager;

        private Book _CurrentBook;
        public Book CurrentBook
        {
            get { return _CurrentBook; }
            set { this.RaiseAndSetIfChanged(ref _CurrentBook, value); }
        }

        //private string _Title;
        //public string Title
        //{
        //    get { return _Title; }
        //    set { this.RaiseAndSetIfChanged(ref _Title, value); }
        //}

        //private string _Authors;
        //public string Authors
        //{
        //    get { return _Authors; }
        //    set { this.RaiseAndSetIfChanged(ref _Authors, value); }
        //}

        //private string _ISBN;
        //public string ISBN
        //{
        //    get { return _ISBN; }
        //    set { this.RaiseAndSetIfChanged(ref _ISBN, value); }
        //}

        //private string _ISBN13;
        //public string ISBN13
        //{
        //    get { return _ISBN13; }
        //    set { this.RaiseAndSetIfChanged(ref _ISBN13, value); }
        //}

        //private string _Description;
        //public string Description
        //{
        //    get { return _Description; }
        //    set { this.RaiseAndSetIfChanged(ref _Description, value); }
        //}

        //private Dictionary<string, string> _Metadata;
        //public Dictionary<string, string> Metadata
        //{
        //    get { return _Metadata; }
        //    set { this.RaiseAndSetIfChanged(ref _Metadata, value); }
        //}

        //private Shelf _SelectedShelf;
        //public Shelf SelectedShelf
        //{
        //    get { return _SelectedShelf; }
        //    set { this.RaiseAndSetIfChanged(ref _SelectedShelf, value); }
        //}

        public ObservableCollectionEx<Shelf> Shelves
        {
            get { return state_manager.CurrentCollection.Shelves; }
        }

        public BookDetailsViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

           
            //this.WhenAnyValue(x => x.CurrentBook)
            //    .Subscribe(Update);
        }

        //private void Update(Book book)
        //{
        //    if (book != null)
        //    {
        //        Title = book.Title;
        //        Authors = string.Join(",", book.Authors);
        //        ISBN = book.ISBN;
        //        ISBN13 = book.ISBN13;
        //        Description = book.Description;
        //        Metadata = book.Metadata;
        //        SelectedShelf = book.Shelf;
        //    }
        //    else
        //    {
        //        Title = string.Empty;
        //        Authors = string.Empty;
        //        ISBN = string.Empty;
        //        ISBN13 = string.Empty;
        //        Description = string.Empty;
        //        Metadata = null;
        //        SelectedShelf = null;
        //    }
        //}
    }
}
