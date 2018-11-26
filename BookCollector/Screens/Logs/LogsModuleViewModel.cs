using BookCollector.Screens.Common;
using NLog;
using Panda.Infrastructure;
using Panda.Utils;
using ReactiveUI;
using System.Collections.ObjectModel;
using System.Linq;

namespace BookCollector.Screens.Logs
{
    public class LogsModuleViewModel : ScreenBase, ILogsModule
    {
        private MemoryTarget log_target;

        public ObservableCollection<string> Messages { get { return log_target.Messages; } }

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

        public LogsModuleViewModel(ApplicationNavigationPartViewModel application_navigation_part,
                                   ToolsNavigationPartViewModel tools_navigation_part)
        {
            ApplicationNavigationPart = application_navigation_part;
            ToolsNavigationPart = tools_navigation_part;

            log_target = LogManager.Configuration.AllTargets.OfType<MemoryTarget>().FirstOrDefault();
        }
    }
}
