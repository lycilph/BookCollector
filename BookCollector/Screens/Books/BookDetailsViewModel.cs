using BookCollector.Application;
using BookCollector.Data;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace BookCollector.Screens.Books
{
    public class BookDetailsViewModel : ScreenBase
    {
        private IStateManager state_manager;
        private Book previous_book;
        private Shelf previous_shelf;

        private Book _CurrentBook;
        public Book CurrentBook
        {
            get { return _CurrentBook; }
            set { this.RaiseAndSetIfChanged(ref _CurrentBook, value); }
        }

        public ObservableCollectionEx<Shelf> Shelves
        {
            get { return state_manager.CurrentCollection.Shelves; }
        }

        public BookDetailsViewModel(IStateManager state_manager)
        {
            this.state_manager = state_manager;

            this.WhenAnyValue(x => x.CurrentBook.Shelf)
                .Subscribe(s =>
                {
                    if (CurrentBook == previous_book)
                    {
                        // Remove book from previous shelf (if possible)
                        if (previous_shelf != null)
                            previous_shelf.Remove(CurrentBook);
                        // Add book to new shelf
                        s.Add(CurrentBook);

                        previous_shelf = s;
                    }
                    else
                    {
                        previous_book = CurrentBook;
                        previous_shelf = CurrentBook.Shelf;
                    }
                });
        }

        public override void OnDeactivated()
        {
            CurrentBook = null;
            previous_book = null;
            previous_shelf = null;
        }
    }
}
