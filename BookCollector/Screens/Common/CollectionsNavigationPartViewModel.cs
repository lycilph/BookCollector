﻿using BookCollector.Application.Messages;
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

        public CollectionsNavigationPartViewModel()
        {
            ShowBooksCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Books));
        }
    }
}