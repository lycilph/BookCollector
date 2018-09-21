using Core.Infrastructure;

namespace BookCollector.Screens.Modules
{
    public interface IModulesViewModel : IModule
    {
        void SetModule(ModuleType module);
    }
}
