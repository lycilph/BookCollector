using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Data;
using BookCollector.Screens.Common;
using NLog;
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
    public class BooksModuleViewModel : CollectionModuleBase, IBooksModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;
        private ISearchEngine<Book> search_engine;
        private List<BookDetailViewModel> books_view_models;
        private List<SearchResult<Book>> search_results;

        private SearchFieldViewModel _SearchField;
        public SearchFieldViewModel SearchField
        {
            get { return _SearchField; }
            set { this.RaiseAndSetIfChanged(ref _SearchField, value); }
        }

        private ShelvesViewModel _Shelves;
        public ShelvesViewModel Shelves
        {
            get { return _Shelves; }
            set { this.RaiseAndSetIfChanged(ref _Shelves, value); }
        }

        private TagsViewModel _Tags;
        public TagsViewModel Tags
        {
            get { return _Tags; }
            set { this.RaiseAndSetIfChanged(ref _Tags, value); }
        }

        private ICollectionView _Books;
        public ICollectionView Books
        {
            get { return _Books; }
            set { this.RaiseAndSetIfChanged(ref _Books, value); }
        }

        public BooksModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part, 
                                    CollectionsNavigationPartViewModel collections_navigation_part, 
                                    ToolsNavigationPartViewModel tools_navigation_part, 
                                    CollectionInformationPartViewModel collection_information_part,
                                    IStateManager state_manager,
                                    ISearchEngine<Book> search_engine,
                                    SearchFieldViewModel search_field,
                                    ShelvesViewModel shelves,
                                    TagsViewModel tags)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            this.state_manager = state_manager;
            this.search_engine = search_engine;
            SearchField = search_field;
            Shelves = shelves;
            Tags = tags;

            this.WhenAnyValue(x => x.SearchField.Text)
                .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(Search);
        }

        public override void OnActivated()
        {
            base.OnActivated(); // CollectionModuleBase handles activation of parts
            Shelves.Activate();
            Tags.Activate();

            // Show message if there are no books in the collection
            if (!state_manager.CurrentCollection.Books.Any())
                MessageBus.Current.SendMessage(new InformationMessage("No books in collection, import here", "Go", () => MessageBus.Current.SendMessage(NavigationMessage.Import)));

            books_view_models = state_manager.CurrentCollection.Books.Select(b => new BookDetailViewModel(b)).ToList();
            Books = CollectionViewSource.GetDefaultView(books_view_models);
            Books.Filter = Filter;

            Books.Events()
                 .WhenAnyObservable(x => x.CurrentChanged)
                 .Subscribe(_ => logger.Trace($"Current book changed to: {((BookDetailViewModel)Books.CurrentItem).Title}"));
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of parts
            Shelves.Deactivate();
            Tags.Deactivate();

            Books = null;
            books_view_models.DisposeAll();
            books_view_models = null;
            search_results = null;
            SearchField.Clear();
        }

        private void Search(string query)
        {
            if (!string.IsNullOrWhiteSpace(query))
            {
                logger.Trace($"Searching for {query}");
                search_results = search_engine.Search(query);
            }
            else
                search_results = null;

            Books?.Refresh();
            Books?.MoveCurrentToFirst();
        }

        private bool Filter(object o)
        {
            var vm = o as BookDetailViewModel;
            if (vm == null)
                return false;

            if (search_results != null)
                return search_results.Any(res => res.Item == vm.Obj);
            else
                return true;
        }
    }
}
