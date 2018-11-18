using BookCollector.Application;
using BookCollector.Application.Messages;
using MaterialDesignThemes.Wpf;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;

namespace BookCollector.Screens.Shell
{
    public class ShellViewModel : ViewAwareScreenBase, IShellViewModel
    {
        private IEnumerable<IModule> modules;

        private IModule _CurrentModule;
        public IModule CurrentModule
        {
            get { return _CurrentModule; }
            set { this.RaiseAndSetIfChanged(ref _CurrentModule, value); }
        }

        private ISnackbarMessageQueue _MessageQueue;
        public ISnackbarMessageQueue MessageQueue
        {
            get { return _MessageQueue; }
            set { this.RaiseAndSetIfChanged(ref _MessageQueue, value); }
        }

        private bool _ShowWindowsCommands;
        public bool ShowWindowsCommands
        {
            get { return _ShowWindowsCommands; }
            set { this.RaiseAndSetIfChanged(ref _ShowWindowsCommands, value); }
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

        public ShellViewModel(IEnumerable<IModule> modules)
        {
            this.modules = modules;

            MessageQueue = new SnackbarMessageQueue(TimeSpan.FromSeconds(1));

            ShowCollectionsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Collections));
            ShowSettingsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(NavigationMessage.Settings));
        }

        public void NavigateTo(Type module)
        {
            var module_to_navigate_to = modules.FirstOrDefault(m => module.IsAssignableFrom(m.GetType()));
            if (module_to_navigate_to == null)
                throw new InvalidOperationException($"Couldn't find module {module}");

            if (CurrentModule != module_to_navigate_to)
                SetCurrentModule(module_to_navigate_to);
        }

        public void ShowMessage(string content)
        {
            MessageQueue.Enqueue(content);
        }

        public void ShowMessage(string content, string action_content, Action action_handler)
        {
            MessageQueue.Enqueue(content, action_content, action_handler);
        }

        public void ShowCommands()
        {
            ShowWindowsCommands = true;
        }

        public void HideCommands()
        {
            ShowWindowsCommands = false;
        }

        protected override void OnViewLoaded(object view)
        {
            MessageBus.Current.SendMessage(ApplicationMessage.ShellLoaded);
        }

        protected override void OnViewUnloaded(object view)
        {
            CurrentModule?.Deactivate();
        }

        private void SetCurrentModule(IModule module)
        {
            CurrentModule?.Deactivate();
            CurrentModule = module;
            CurrentModule?.Activate();
        }
    }
}
