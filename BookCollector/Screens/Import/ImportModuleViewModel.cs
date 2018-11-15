using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Import
{
    public class ImportModuleViewModel : ScreenBase, IImportModule
    {
        private ReactiveCommand<Unit, Unit> _BackCommand;
        public ReactiveCommand<Unit, Unit> BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public ImportModuleViewModel()
        {
            BackCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Collections));
        }
    }
}