using BookCollector.Application;
using Core;
using Core.Application;
using Core.Data;
using Core.Infrastructure;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Collections.Generic;
using System.Linq;

namespace BookCollector.Screens.Modules
{
    public class ModulesViewModel : ScreenBase, IModulesViewModel
    {
        private IStateManager state_manager;
        private IEnumerable<IModule> Modules;

        public ModuleType Type { get; } = ModuleType.Modules;

        private Collection _CurrentCollection;
        public Collection CurrentCollection
        {
            get { return _CurrentCollection; }
            set { this.RaiseAndSetIfChanged(ref _CurrentCollection, value); }
        }

        private ReactiveList<IModule> _CollectionModules;
        public ReactiveList<IModule> CollectionModules
        {
            get { return _CollectionModules; }
            set { this.RaiseAndSetIfChanged(ref _CollectionModules, value); }
        }

        private ReactiveList<IModule> _PluginModules;
        public ReactiveList<IModule> PluginModules
        {
            get { return _PluginModules; }
            set { this.RaiseAndSetIfChanged(ref _PluginModules, value); }
        }

        private IModule _CurrentModule;
        public IModule CurrentModule
        {
            get { return _CurrentModule; }
            set
            {
                if (_CurrentModule != value)
                {
                    _CurrentModule?.Deactivate();
                    this.RaiseAndSetIfChanged(ref _CurrentModule, value);
                    _CurrentModule?.Activate();
                }
            }
        }

        private ReactiveCommand _CollectionsCommand;
        public ReactiveCommand CollectionsCommand
        {
            get { return _CollectionsCommand; }
            set { this.RaiseAndSetIfChanged(ref _CollectionsCommand, value); }
        }

        private ReactiveCommand _SettingsCommand;
        public ReactiveCommand SettingsCommand
        {
            get { return _SettingsCommand; }
            set { this.RaiseAndSetIfChanged(ref _SettingsCommand, value); }
        }

        public ModulesViewModel(IStateManager state_manager, IEnumerable<IModule> Modules)
        {
            DisplayName = "Modules";
            this.state_manager = state_manager;
            this.Modules = Modules;

            CollectionModules = Modules.Where(m => Constants.CollectionModules.Contains(m.Type))
                                       .ToReactiveList();

            PluginModules = Modules.Where(m => Constants.PluginModules.Contains(m.Type))
                                   .ToReactiveList();

            CollectionsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.NavigateToCollections));
            SettingsCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.NavigateToSettings));
        }

        public override void OnActivated()
        {
            CurrentCollection = state_manager.CurrentCollection;
            CurrentModule?.Activate();
        }

        public override void OnDeactivated()
        {
            CurrentModule?.Deactivate();
            CurrentCollection = null;
        }

        public void SetModule(ModuleType module)
        {
            CurrentModule = Modules.FirstOrDefault(m => m.Type == module);
        }
    }
}
