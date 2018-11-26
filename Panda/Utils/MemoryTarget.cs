using NLog;
using NLog.Targets;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace Panda.Utils
{
    // http://dotnetsolutionsbytomi.blogspot.com/2011/06/creating-awesome-logging-control-with.html
    // https://stackoverflow.com/questions/6617689/how-can-i-use-a-richtextbox-as-a-nlog-target-in-a-wpf-application
    public class MemoryTarget : TargetWithLayout
    {
        private object messages_lock = new object();

        public ObservableCollection<string> Messages { get; } = new ObservableCollection<string>();

        public MemoryTarget()
        {
            // This handles the dispatching to the UI thread (so that WPF doesn't complain about changes from non-ui threads)
            BindingOperations.EnableCollectionSynchronization(Messages, messages_lock);
        }

        protected override void Write(LogEventInfo log_event)
        {
            var message = Layout.Render(log_event);
            Messages.Add(message);
        }
    }
}
