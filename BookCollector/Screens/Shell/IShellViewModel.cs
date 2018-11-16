using System;

namespace BookCollector.Screens.Shell
{
    public interface IShellViewModel
    {
        void NavigateTo(Type module);

        void ShowCommands();
        void HideCommands();
    }
}
