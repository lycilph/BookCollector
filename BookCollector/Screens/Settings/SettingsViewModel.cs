using BookCollector.Application;
using Core.Application;
using Core.Infrastructure;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    public class SettingsViewModel : ScreenBase, IModule
    {
        private IStateManager state_manager;

        public ModuleType Type { get; } = ModuleType.Settings;

        public bool LoadOnStart
        {
            get { return state_manager.Settings.LoadMostRecentCollectionOnStart; }
            set { state_manager.Settings.LoadMostRecentCollectionOnStart = value; }
        }

        private ReactiveCommand _BackCommand;
        public ReactiveCommand BackCommand
        {
            get { return _BackCommand; }
            set { this.RaiseAndSetIfChanged(ref _BackCommand, value); }
        }

        public SettingsViewModel(IStateManager state_manager)
        {
            DisplayName = "Settings";
            this.state_manager = state_manager;

            BackCommand = ReactiveCommand.Create(() => MessageBus.Current.SendMessage(ApplicationMessage.NavigateToModules));
        }
    }
}
