using ReactiveUI;
using System;
using System.Collections.Generic;

namespace Panda.Infrastructure
{
    public class ScreenBase : ReactiveObject, IScreen, IEquatable<ScreenBase>
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

        public virtual void OnActivated() { }

        public void Deactivate()
        {
            IsActive = false;
            OnDeactivated();
        }

        public virtual void OnDeactivated() { }

        public override bool Equals(object obj)
        {
            return Equals(obj as ScreenBase);
        }

        public bool Equals(ScreenBase other)
        {
            return other != null &&
                   DisplayName == other.DisplayName;
        }

        public override int GetHashCode()
        {
            return 1862586150 + EqualityComparer<string>.Default.GetHashCode(DisplayName);
        }
    }
}
