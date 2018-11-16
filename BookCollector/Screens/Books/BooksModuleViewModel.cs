using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using ReactiveUI;

namespace BookCollector.Screens.Books
{
    public class BooksModuleViewModel : CollectionModuleBase, IBooksModule
    {
        public BooksModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part, 
                                    CollectionsNavigationPartViewModel collections_navigation_part, 
                                    ToolsNavigationPartViewModel tools_navigation_part, 
                                    CollectionInformationPartViewModel collection_information_part) 
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
        }

        public override void OnActivated()
        {
            // Show windows commands
            MessageBus.Current.SendMessage(ApplicationMessage.ShowCommands);
        }
    }
}
