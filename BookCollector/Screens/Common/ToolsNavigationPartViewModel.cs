﻿using BookCollector.Application.Messages;
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

        private ReactiveCommand<Unit, Unit> _ShowLogsCommand;
        public ReactiveCommand<Unit, Unit> ShowLogsCommand
        {
            get { return _ShowLogsCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowLogsCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ShowToolsCommand;
        public ReactiveCommand<Unit, Unit> ShowToolsCommand
        {
            get { return _ShowToolsCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowToolsCommand, value); }
        }

        public ToolsNavigationPartViewModel()
        {
            ShowImportCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Import));
            ShowLogsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Logs));
            ShowToolsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Tools));
        }
    }
}
