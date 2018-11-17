using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using BookCollector.Screens.Controls;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksModuleViewModel : CollectionModuleBase, IBooksModule
    {
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
                                    SearchFieldViewModel search_field) 
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            SearchField = search_field;
        }

        public override void OnActivated()
        {
            base.OnActivated(); // CollectionModuleBase handles activation or parts

            // Show windows commands
            MessageBus.Current.SendMessage(ApplicationMessage.ShowCommands);
        }
    }
}
