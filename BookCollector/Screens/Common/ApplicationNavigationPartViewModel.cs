using Panda.Infrastructure;
using ReactiveUI;

namespace BookCollector.Screens.Common
{
    public class ApplicationNavigationPartViewModel : ScreenBase
    {
        private bool _IsOpen;
        public bool IsOpen
        {
            get { return _IsOpen; }
            set { this.RaiseAndSetIfChanged(ref _IsOpen, value); }
        }
    }
}
