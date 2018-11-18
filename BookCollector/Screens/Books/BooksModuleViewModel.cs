using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using Panda.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace BookCollector.Screens.Books
{
    public class BooksModuleViewModel : CollectionModuleBase, IBooksModule
    {
        private IStateManager state_manager;
        private List<BookDetailViewModel> books_view_models;

        private SearchFieldViewModel _SearchField;
        public SearchFieldViewModel SearchField
        {
            get { return _SearchField; }
            set { this.RaiseAndSetIfChanged(ref _SearchField, value); }
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
                                    SearchFieldViewModel search_field)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            this.state_manager = state_manager;
            SearchField = search_field;
        }

        public override void OnActivated()
        {
            base.OnActivated(); // CollectionModuleBase handles activation of parts

            // Show message if there are no books in the collection
            if (!state_manager.CurrentCollection.Books.Any())
                MessageBus.Current.SendMessage(new InformationMessage("No books in collection, import here", "Go", () => MessageBus.Current.SendMessage(NavigationMessage.Import)));

            books_view_models = state_manager.CurrentCollection.Books.Select(b => new BookDetailViewModel(b)).ToList();
            Books = CollectionViewSource.GetDefaultView(books_view_models);
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of parts

            Books = null;
            books_view_models.DisposeAll();
            books_view_models = null;
        }
    }
}
