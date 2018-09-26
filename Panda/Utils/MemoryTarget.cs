using NLog;
using NLog.Targets;
using ReactiveUI;

namespace Panda.Utils
{
    // http://dotnetsolutionsbytomi.blogspot.com/2011/06/creating-awesome-logging-control-with.html
    // https://stackoverflow.com/questions/6617689/how-can-i-use-a-richtextbox-as-a-nlog-target-in-a-wpf-application
    public class MemoryTarget : TargetWithLayout
    {
        public ReactiveList<string> Messages { get; private set; } = new ReactiveList<string>();

        protected override void Write(LogEventInfo log_event)
        {
            var message = Layout.Render(log_event);
            Messages.Add(message);
        }
    }
}
