using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Common
{
    public class CollectionsNavigationPartViewModel : ScreenBase
    {
        private ReactiveCommand<Unit, Unit> _ShowBooksCommand;
        public ReactiveCommand<Unit, Unit> ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ShowSeriesCommand;
        public ReactiveCommand<Unit, Unit> ShowSeriesCommand
        {
            get { return _ShowSeriesCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowSeriesCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ShowNotesCommand;
        public ReactiveCommand<Unit, Unit> ShowNotesCommand
        {
            get { return _ShowNotesCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowNotesCommand, value); }
        }

        public CollectionsNavigationPartViewModel()
        {
            ShowBooksCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Books));
            ShowSeriesCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Series));
            ShowNotesCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Notes));
        }
    }
}
