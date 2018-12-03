using BookCollector.Screens.Common;
using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Tools
{
    public class ToolsModuleViewModel : ScreenBase, IToolsModule
    {
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

        public ToolsModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                    ToolsNavigationPartViewModel tools_navigation_part)
        {
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;
        }
    }
}
