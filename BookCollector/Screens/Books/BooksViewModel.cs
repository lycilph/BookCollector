using Core.Application;
using Core.Data;
using Core.Infrastructure;
using NLog;
using Panda.Infrastructure;
using Panda.Search;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

namespace BookCollector.Screens.Books
{
    public class BooksViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private ISearchEngine<Book> search_engine;
        private List<BookViewModel> books_view_models;
        private List<SearchResult<Book>> search_results;

        public ModuleType Type { get; } = ModuleType.Books;
        
        private ICollectionView _Books;
        public ICollectionView Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        private string _SearchText;
        public string SearchText
        {
            get { return _SearchText; }
            set { this.RaiseAndSetIfChanged(ref _SearchText, value); }
        }

        private ReactiveCommand _ClearCommand;
        public ReactiveCommand ClearCommand
        {
            get { return _ClearCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearCommand, value); }
        }

        public BooksViewModel(IStateManager state_manager, ISearchEngine<Book> search_engine)
        {
            DisplayName = "Books";
            this.state_manager = state_manager;
            this.search_engine = search_engine;

            this.WhenAnyValue(x => x.SearchText)
                .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(_ => Search(SearchText));

            ClearCommand = ReactiveCommand.Create(() => SearchText = string.Empty);
        }

        public override void OnActivated()
        {
            books_view_models = state_manager.CurrentCollection.Books.Select(b => new BookViewModel(b)).ToList();
            Books = CollectionViewSource.GetDefaultView(books_view_models);
            Books.Filter = Filter;

            var books_list = state_manager.CurrentCollection.Books.ToList();
            search_engine.Index(books_list, book => book.Title + " " + string.Join(" ", book.Authors));
        }

        public override void OnDeactivated()
        {
            Books = null;
            books_view_models.DisposeAll();
            books_view_models = null;
            search_results = null;
        }

        private void Search(string query)
        {
            logger.Trace($"Searching for {query}");
            search_results = search_engine.Search(query);

            Books?.Refresh();
            Books?.MoveCurrentToFirst();
        }

        private bool Filter(object o)
        {
            var vm = o as BookViewModel;
            if (vm == null)
                return false;

            if (search_results != null)
                return search_results.Any(res => res.Item == vm.Obj);
            else
                return true;
        }
    }
}
