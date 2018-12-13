using BookCollector.Application;
using BookCollector.Screens.Common;
using Panda.Utils;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookCollector.Screens.Series
{
    public class SeriesModuleViewModel : CollectionModuleBase, ISeriesModule
    {
        private IStateManager state_manager;

        private ObservableCollection<SeriesViewModel> _Series;
        public ObservableCollection<SeriesViewModel> Series
        {
            get { return _Series; }
            set { this.RaiseAndSetIfChanged(ref _Series, value); }
        }

        private SeriesViewModel _SelectedSeries;
        public SeriesViewModel SelectedSeries
        {
            get { return _SelectedSeries; }
            set { this.RaiseAndSetIfChanged(ref _SelectedSeries, value); }
        }

        public SeriesModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                     CollectionsNavigationPartViewModel collections_navigation_part,
                                     ToolsNavigationPartViewModel tools_navigation_part,
                                     CollectionInformationPartViewModel collection_information_part,
                                     IStateManager state_manager)
            : base(application_navigation_part, collections_navigation_part, tools_navigation_part, collection_information_part)
        {
            this.state_manager = state_manager;
        }

        public override void OnActivated()
        {
            base.OnActivated(); // CollectionModuleBase handles activation of common parts

            Series = state_manager.CurrentCollection.Series.Select(s => new SeriesViewModel(s)).ToObservableCollection();
            SelectedSeries = Series.FirstOrDefault();
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of common parts

            Series.DisposeAll();
            Series = null;
            SelectedSeries = null;
        }
    }
}
