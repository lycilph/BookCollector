using Core.Application;
using Core.Infrastructure;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : ScreenBase, IModule
    {
        private IStateManager state_manager;

        public ModuleType Type { get; } = ModuleType.Books;

        private ReactiveList<BookViewModel> _Books;
        public ReactiveList<BookViewModel> Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private BookViewModel _SelectedBook;
        public BookViewModel SelectedBook
        {
            get { return _SelectedBook; }
            set { this.RaiseAndSetIfChanged(ref _SelectedBook, value); }
        }

        public BooksViewModel(IStateManager state_manager)
        {
            DisplayName = "Books";
            this.state_manager = state_manager;
        }

        public override void OnActivated()
        {
            Books = state_manager.CurrentCollection.Books.Select(b => new BookViewModel(b)).ToReactiveList();
            SelectedBook = Books.FirstOrDefault();
        }

        public override void OnDeactivated()
        {
            Books.DisposeAll();
            Books = null;
            SelectedBook = null;
        }
    }
}
