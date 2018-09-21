using Core.Infrastructure;

namespace BookCollector.Screens.Shell
{
    public interface IShellViewModel
    {
        void NavigateTo(ModuleType module);
    }
}
