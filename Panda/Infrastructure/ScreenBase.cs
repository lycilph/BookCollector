using ReactiveUI;

namespace Panda.Infrastructure
{
    public class ScreenBase : ReactiveObject, IScreen
    {
        private string _DisplayName;
        public string DisplayName
        {
            get { return _DisplayName; }
            set { this.RaiseAndSetIfChanged(ref _DisplayName, value); }
        }

        private bool _IsActive;
        public bool IsActive
        {
            get { return _IsActive; }
            set { this.RaiseAndSetIfChanged(ref _IsActive, value); }
        }

        public void Activate()
        {
            IsActive = true;
            OnActivated();
        }

        public void Deactivate()
        {
            IsActive = false;
            OnDeactivated();
        }

        public virtual void OnActivated() { }

        public virtual void OnDeactivated() { }
    }
}
