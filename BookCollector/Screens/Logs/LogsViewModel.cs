using Core.Infrastructure;
using NLog;
using Panda.Utils;
using Panda.Infrastructure;
using ReactiveUI;
using System.Linq;

namespace BookCollector.Screens.Logs
{
    public class LogsViewModel : ScreenBase, IModule
    {
        private MemoryTarget log_target;

        public ModuleType Type { get; } = ModuleType.Logs;
        public ReactiveList<string> Messages { get { return log_target.Messages; } }

        public LogsViewModel()
        {
            DisplayName = "Logs";
            log_target = LogManager.Configuration.AllTargets.OfType<MemoryTarget>().FirstOrDefault();
        }
    }
}
