﻿using ReactiveUI;

namespace BookCollector.Dialogs
{
    public class PromptDialogViewModel : ReactiveObject
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
    }
}
