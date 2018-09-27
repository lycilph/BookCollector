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

        private ReactiveList<ModuleGroupViewModel> _GroupedModules;
        public ReactiveList<ModuleGroupViewModel> GroupedModules
        {
            get { return _GroupedModules; }
            set { this.RaiseAndSetIfChanged(ref _GroupedModules, value); }
        }

        private ModuleGroupViewModel _CurrentModule;
        public ModuleGroupViewModel CurrentModule
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

            var collection_modules = Modules.Where(m => Constants.CollectionModules.Contains(m.Type))
                                            .Select(m => new ModuleGroupViewModel(m, "Collection"));
            var plugin_modules = Modules.Where(m => Constants.PluginModules.Contains(m.Type))
                                        .Select(m => new ModuleGroupViewModel(m, "Plugin"));
            GroupedModules = collection_modules.Concat(plugin_modules)
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
            CurrentModule = GroupedModules.FirstOrDefault(m => m.Type == module);
        }
    }
}
