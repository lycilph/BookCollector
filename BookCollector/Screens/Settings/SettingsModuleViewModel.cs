using BookCollector.Application;
using BookCollector.Screens.Common;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Settings
{
    public class SettingsModuleViewModel : ScreenBase, ISettingsModule
    {
        private IStateManager state_manager;

        public bool LoadOnStart
        {
            get { return state_manager.Settings.LoadMostRecentCollectionOnStart; }
            set { state_manager.Settings.LoadMostRecentCollectionOnStart = value; }
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
    }
}
