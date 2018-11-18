using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Books
{
    public class BooksModuleViewModel : CollectionModuleBase, IBooksModule
    {
        private IStateManager state_manager;

        private SearchFieldViewModel _SearchField;
        public SearchFieldViewModel SearchField
        {
            get { return _SearchField; }
            set { this.RaiseAndSetIfChanged(ref _SearchField, value); }
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
            base.OnActivated(); // CollectionModuleBase handles activation or parts

            // Show message if there are no books in the collection
            if (!state_manager.CurrentCollection.Books.Any())
                MessageBus.Current.SendMessage(new InformationMessage("No books in collection, import here", "Go", () => MessageBus.Current.SendMessage(NavigationMessage.Import)));
        }
    }
}
