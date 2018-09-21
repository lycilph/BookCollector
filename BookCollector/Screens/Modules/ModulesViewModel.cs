using Core.Infrastructure;
using Panda.Infrastructure;
using ReactiveUI;
using System.Collections.Generic;

namespace BookCollector.Screens.Modules
{
    public class ModulesViewModel : ScreenBase, IModulesViewModel
    {
        public ModuleType Type { get; } = ModuleType.Modules;
        public IEnumerable<IModule> Modules { get; set; }

        private IModule _CurrentModule;
        public IModule CurrentModule
        {
            get { return _CurrentModule; }
            set
            {
                if (_CurrentModule != value)
                {
                    _CurrentModule?.Deactivate();
                    this.RaiseAndSetIfChanged(ref _CurrentModule, value);
                    _CurrentModule?.Activate();
                }
            }
        }

        public ModulesViewModel()
        {
            DisplayName = "Modules";
        }

        public override void OnActivated()
        {
            CurrentModule?.Activate();
        }

        public override void OnDeactivated()
        {
            CurrentModule?.Deactivate();
        }

        public void SetModule(ModuleType module)
        {
        }
    }
}
