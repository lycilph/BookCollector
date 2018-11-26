using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System.Reactive;

namespace BookCollector.Screens.Common
{
    public class ApplicationNavigationPartViewModel : ScreenBase
    {
        private bool _IsOpen;
        public bool IsOpen
        {
            get { return _IsOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsOpen, value); }
        }

        private ReactiveCommand<Unit, Unit> _ShowBooksCommand;
        public ReactiveCommand<Unit, Unit> ShowBooksCommand
        {
            get { return _ShowBooksCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowBooksCommand, value); }
        }

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

        private ReactiveCommand<Unit, Unit> _ShowCollectionsCommand;
        public ReactiveCommand<Unit, Unit> ShowCollectionsCommand
        {
            get { return _ShowCollectionsCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowCollectionsCommand, value); }
        }

        private ReactiveCommand<Unit, Unit> _ShowSettingsCommand;
        public ReactiveCommand<Unit, Unit> ShowSettingsCommand
        {
            get { return _ShowSettingsCommand; }
            set { this.RaiseAndSetIfChanged(ref _ShowSettingsCommand, value); }
        }

        public ApplicationNavigationPartViewModel()
        {
            ShowBooksCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                MessageBus.Current.SendMessage(NavigationMessage.Books);
            });

            ShowImportCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                MessageBus.Current.SendMessage(NavigationMessage.Import);
            });

            ShowLogsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                MessageBus.Current.SendMessage(NavigationMessage.Logs);
            });

            ShowCollectionsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                MessageBus.Current.SendMessage(NavigationMessage.Collections);
            });

            ShowSettingsCommand = ReactiveCommand.Create(() =>
            {
                IsOpen = false;
                MessageBus.Current.SendMessage(NavigationMessage.Settings);
            });
        }
    }
}
