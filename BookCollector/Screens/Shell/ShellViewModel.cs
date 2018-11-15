using BookCollector.Application;
using BookCollector.Application.Messages;
using Panda.Infrastructure;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public ShellViewModel(IEnumerable<IModule> modules)
        {
            this.modules = modules;
        }

        public void NavigateTo(Type module)
        {
            var module_to_navigate_to = modules.FirstOrDefault(m => module.IsAssignableFrom(m.GetType()));
            if (module_to_navigate_to == null)
                throw new InvalidOperationException($"Couldn't find module {module}");

            SetCurrentModule(module_to_navigate_to);
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
