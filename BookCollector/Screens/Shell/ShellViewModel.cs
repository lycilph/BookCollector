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
            var temp = modules.FirstOrDefault(m => module.IsAssignableFrom(m.GetType()));
            CurrentModule = temp ?? throw new InvalidOperationException($"Couldn't find module {module}");
        }

        protected override void OnViewLoaded(object view)
        {
            MessageBus.Current.SendMessage(ApplicationMessage.ShellLoaded);
        }
    }
}
