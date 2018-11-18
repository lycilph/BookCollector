using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Books
{
    public class SearchFieldViewModel : ReactiveObject
    {
        private string _Text;
        public string Text
        {
            get { return _Text; }
            set { this.RaiseAndSetIfChanged(ref _Text, value); }
        }

        private ReactiveCommand<Unit, Unit> _ClearCommand;
        public ReactiveCommand<Unit, Unit> ClearCommand
        {
            get { return _ClearCommand; }
            set { this.RaiseAndSetIfChanged(ref _ClearCommand, value); }
        }

        public SearchFieldViewModel()
        {
            ClearCommand = ReactiveCommand.Create(() => { Text = string.Empty; });
        }
    }
}
