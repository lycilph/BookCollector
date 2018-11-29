using BookCollector.Application;
using BookCollector.Data;
using Panda.Collections;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BookDetailsViewModel : ScreenBase
    {
        private IStateManager state_manager;

        private object obj = new object();

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
        }

        public override void OnDeactivated()
        {
            CurrentBook = null;
        }
    }
}
