using ReactiveUI;

namespace BookCollector.Screens.Dialogs
{
    public class InputDialogViewModel : ReactiveObject
    {
        private string _Title;
        public string Title
        {
            get { return _Title; }
            set { this.RaiseAndSetIfChanged(ref _Title, value); }
        }

        private string _Message;
        public string Message
        {
            get { return _Message; }
            set { this.RaiseAndSetIfChanged(ref _Message, value); }
        }

        private string _Hint;
        public string Hint
        {
            get { return _Hint; }
            set { this.RaiseAndSetIfChanged(ref _Hint, value); }
        }

        private string _Input;
        public string Input
        {
            get { return _Input; }
            set { this.RaiseAndSetIfChanged(ref _Input, value); }
        }
    }
}
