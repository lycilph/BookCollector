using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Books
{
    public class BooksModuleViewModel : ScreenBase, IBooksModule
    {
        private ReactiveCommand<Unit, Unit> _BackCommand;
        public ReactiveCommand<Unit, Unit> BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public BooksModuleViewModel()
        {
            BackCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Collections));
        }
    }
}
