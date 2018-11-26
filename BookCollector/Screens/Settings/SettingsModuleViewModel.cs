using BookCollector.Application;
using BookCollector.Application.Messages;
using BookCollector.Screens.Common;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    public class SettingsModuleViewModel : ScreenBase, ISettingsModule
    {
        private IStateManager state_manager;
        private int current_snackbar_message_duration;

        public bool LoadOnStart
        {
            get { return state_manager.Settings.LoadMostRecentCollectionOnStart; }
            set { state_manager.Settings.LoadMostRecentCollectionOnStart = value; }
        }

        public int SnackbarMessageDuration
        {
            get { return state_manager.Settings.SnackbarMessageDuration; }
            set
            {
                state_manager.Settings.SnackbarMessageDuration = value;
                this.RaisePropertyChanged();
            }
        }

        public bool EnableShelfMatching
        {
            get { return state_manager.Settings.EnableShelfMatching; }
            set
            {
                state_manager.Settings.EnableShelfMatching = value;
                this.RaisePropertyChanged();
            }
        }

        public bool CreateUnmatchedShelves
        {
            get { return state_manager.Settings.CreateUnmatchedShelves; }
            set { state_manager.Settings.CreateUnmatchedShelves = value; }
        }

        public int MaxEditDistance
        {
            get { return state_manager.Settings.MaxEditDistanceForShelfMatching; }
            set
            {
                state_manager.Settings.MaxEditDistanceForShelfMatching = value;
                this.RaisePropertyChanged();
            }
        }

        private ApplicationNavigationPartViewModel _ApplicationNavigationPart;
        public ApplicationNavigationPartViewModel ApplicationNavigationPart
        {
            get { return _ApplicationNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ApplicationNavigationPart, value); }
        }

        private ToolsNavigationPartViewModel _ToolsNavigationPart;
        public ToolsNavigationPartViewModel ToolsNavigationPart
        {
            get { return _ToolsNavigationPart; }
            set { this.RaiseAndSetIfChanged(ref _ToolsNavigationPart, value); }
        }

        public SettingsModuleViewModel(IStateManager state_manager,
                                       ApplicationNavigationPartViewModel application_navigation_part,
                                       ToolsNavigationPartViewModel tools_navigation_part)
        {
            this.state_manager = state_manager;
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;
        }

        public override void OnActivated()
        {
            current_snackbar_message_duration = state_manager.Settings.SnackbarMessageDuration;
        }

        public override void OnDeactivated()
        {
            // Send message if duration has changed
            if (current_snackbar_message_duration != state_manager.Settings.SnackbarMessageDuration)
                MessageBus.Current.SendMessage(ApplicationMessage.SnackbarMessageDurationUpdated);
        }
    }
}
