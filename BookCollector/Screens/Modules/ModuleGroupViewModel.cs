using Core.Infrastructure;
using Panda.Infrastructure;

namespace BookCollector.Screens.Modules
{
    public class ModuleGroupViewModel : ItemViewModel<IModule>, IScreen
    {
        public string Group { get; set; }

        public string DisplayName
        {
            get { return Obj.DisplayName; }
            set { Obj.DisplayName = value; }
        }

        public bool IsActive { get { return Obj.IsActive; } }

        public ModuleType Type { get { return Obj.Type; } }

        public ModuleGroupViewModel(IModule obj, string group) : base(obj)
        {
            Group = group;
        }

        public void Activate()
        {
            Obj.Activate();
        }

        public void Deactivate()
        {
            Obj.Deactivate();
        }
    }
}
