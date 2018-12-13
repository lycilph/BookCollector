using BookCollector.Application;
using BookCollector.Screens.Common;
using Panda.Utils;
using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookCollector.Screens.Series
{
    public class SeriesModuleViewModel : CollectionModuleBase, ISeriesModule
    {
        private IStateManager state_manager;
        private IDisposable collection_subscription;

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

            collection_subscription = state_manager.CurrentCollection.Series.SomethingChanged.Subscribe(_ => Update());
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated(); // CollectionModuleBase handles deactivation of common parts

            collection_subscription.Dispose();
            collection_subscription = null;

            Series.DisposeAll();
            Series = null;
            SelectedSeries = null;
        }

        private void Update()
        {
            var selected_series = SelectedSeries?.Obj;

            Series.DisposeAll();
            Series = state_manager.CurrentCollection.Series.Select(s => new SeriesViewModel(s)).ToObservableCollection();
            SelectedSeries = Series.FirstOrDefault(s => s.Metadata["GoodreadsSeriesId"] == selected_series?.Metadata["GoodreadsSeriesId"]);

            // If selected_series was null, we will end up here
            if (SelectedSeries == null)
                SelectedSeries = Series.FirstOrDefault();
        }
    }
}
