using Core.Application;
using Core.Infrastructure;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Series
{
    public class SeriesViewModel : ScreenBase, IModule
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private IStateManager state_manager;

        public ModuleType Type { get; } = ModuleType.Series;

        private ReactiveList<SeriesDetailViewModel> _Series;
        public ReactiveList<SeriesDetailViewModel> Series
        {
            get { return _Series; }
            set { this.RaiseAndSetIfChanged(ref _Series, value); }
        }

        private SeriesDetailViewModel _SelectedSeries;
        public SeriesDetailViewModel SelectedSeries
        {
            get { return _SelectedSeries; }
            set { this.RaiseAndSetIfChanged(ref _SelectedSeries, value); }
        }

        public SeriesViewModel(IStateManager state_manager)
        {
            DisplayName = "Series";
            this.state_manager = state_manager;
        }

        public override void OnActivated()
        {
            Series = state_manager.CurrentCollection.Series.Select(s => new SeriesDetailViewModel(s)).ToReactiveList();
            SelectedSeries = Series.FirstOrDefault();
        }

        public override void OnDeactivated()
        {
            Series.DisposeAll();
            Series = null;
            SelectedSeries = null;
        }
    }
}
