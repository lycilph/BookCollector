using BookCollector.Screens.Common;

namespace BookCollector.Screens.Series
{
    public class SeriesModuleViewModel : CollectionModuleBase, ISeriesModule
    {
        public SeriesModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                     CollectionsNavigationPartViewModel collections_navigation_part,
                                     ToolsNavigationPartViewModel tools_navigation_part,
                                     CollectionInformationPartViewModel collection_information_part)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
        }
    }
}
