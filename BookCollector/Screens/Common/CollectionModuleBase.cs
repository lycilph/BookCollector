using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Common
{
    public class CollectionModuleBase : ScreenBase
    {
        private ApplicationNavigationPartViewModel _ApplicationNavigationPart;
        public ApplicationNavigationPartViewModel ApplicationNavigationPart
        {
            get { return _ApplicationNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ApplicationNavigationPart, value); }
        }

        private CollectionsNavigationPartViewModel _CollectionNavigationPart;
        public CollectionsNavigationPartViewModel CollectionNavigationPart
        {
            get { return _CollectionNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _CollectionNavigationPart, value); }
        }

        private ToolsNavigationPartViewModel _ToolsNavigationPart;
        public ToolsNavigationPartViewModel ToolsNavigationPart
        {
            get { return _ToolsNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ToolsNavigationPart, value); }
        }

        private CollectionInformationPartViewModel _CollectionInformationPart;
        public CollectionInformationPartViewModel CollectionInformationPart
        {
            get { return _CollectionInformationPart; }
            set { this.RaiseAndSetIfChanged(ref _CollectionInformationPart, value); }
        }

        public CollectionModuleBase(ApplicationNavigationPartViewModel application_navigation_part,
                                    CollectionsNavigationPartViewModel collections_navigation_part,
                                    ToolsNavigationPartViewModel tools_navigation_part,
                                    CollectionInformationPartViewModel collection_information_part)
        {
            ApplicationNavigationPart = application_navigation_part;
            CollectionNavigationPart = collections_navigation_part;
            ToolsNavigationPart = tools_navigation_part;
            CollectionInformationPart = collection_information_part;
        }
    }
}
