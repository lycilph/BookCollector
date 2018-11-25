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
        private List<SearchResult<Book>> search_results;
        private List<BookViewModel> book_view_models;
        private IDisposable current_book_disposable;
        private List<ShelfViewModel> selected_shelves;

        private SearchFieldViewModel _SearchField;
        public SearchFieldViewModel SearchField
        {
            get { return _SearchField; }
            set { this.RaiseAndSetIfChanged(ref _SearchField, value); }
        }

        private ShelvesViewModel _ShelvesList;
        public ShelvesViewModel ShelvesList
        {
            get { return _ShelvesList; }
            set { this.RaiseAndSetIfChanged(ref _ShelvesList, value); }
        }

        private BookDetailsViewModel _BookDetails;
        public BookDetailsViewModel BookDetails
        {
            get { return _BookDetails; }
            set { this.RaiseAndSetIfChanged(ref _BookDetails, value); }
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
                                    BookDetailsViewModel book_details,
                                    ShelvesViewModel shelves_list)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            this.state_manager = state_manager;
            this.search_engine = search_engine;
            SearchField = search_field;
            BookDetails = book_details;
            ShelvesList = shelves_list;

            this.WhenAnyValue(x => x.SearchField.Text)
                .Throttle(TimeSpan.FromMilliseconds(250), RxApp.MainThreadScheduler)
                .Subscribe(Search);

            (this).WhenAnyObservable(x => x.ShelvesList.Shelves.SomethingChanged)
                .Subscribe(_ => UpdateSelectedShelves());
        }

        public override void OnActivated()
        {
            base.OnActivated();  // CollectionModuleBase handles activation of common parts
            ShelvesList.Activate();
            BookDetails.Activate();

            UpdateSelectedShelves();

            // Show message if there are no books in the collection
            if (!state_manager.CurrentCollection.Books.Any())
                MessageBus.Current.SendMessage(new InformationMessage("No books in collection, import here", "Go", () => MessageBus.Current.SendMessage(NavigationMessage.Import)));

            book_view_models = state_manager.CurrentCollection
                                            .Books
                                            .Select(b => new BookViewModel(b))
                                            .ToList();

            Books = CollectionViewSource.GetDefaultView(book_view_models);
            Books.Filter = Filter;

            current_book_disposable = Books.Events()
                                           .WhenAnyObservable(x => x.CurrentChanged)
                                           .Select(_ => Books.CurrentItem as BookViewModel)
                                           .Subscribe(vm => BookDetails.CurrentBook = vm?.Obj);


            // This is needed to update the book details
            Books.Refresh();
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of common parts
            ShelvesList.Deactivate();
            BookDetails.Deactivate();

            selected_shelves = null;
            current_book_disposable.Dispose();
            current_book_disposable = null;
            SearchField.Clear(); // This will also clear the search_results
            Books = null;
            book_view_models.DisposeAll();
            book_view_models = null;
        }

        private void UpdateSelectedShelves()
        {
            selected_shelves = ShelvesList.Shelves.Where(s => s.Selected).ToList();
            Books?.Refresh();
            Books?.MoveCurrentToFirst();
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
            var vm = o as BookViewModel;
            if (vm == null)
                return false;

            if (!selected_shelves.Any(s => s.Obj == vm.Obj.Shelf))
                return false;

            if (search_results != null)
                return search_results.Any(res => res.Item == vm.Obj);
            else
                return true;
        }
    }
}
