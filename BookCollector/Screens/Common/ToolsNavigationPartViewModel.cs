using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Common
{
    public class ToolsNavigationPartViewModel : ScreenBase
    {
        private ReactiveCommand<Unit, Unit> _ShowImportCommand;
        public ReactiveCommand<Unit, Unit> ShowImportCommand
        {
            get { return _ShowImportCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowImportCommand, value); }
        }

        public ToolsNavigationPartViewModel()
        {
            ShowImportCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Import));
        }
    }
}
