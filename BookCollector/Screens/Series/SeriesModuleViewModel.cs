using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Series
{
    public class SeriesModuleViewModel : ScreenBase, ISeriesModule
    {
        private ReactiveCommand<Unit, Unit> _BackCommand;
        public ReactiveCommand<Unit, Unit> BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public SeriesModuleViewModel()
        {
            BackCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Collections));
        }
    }
}
