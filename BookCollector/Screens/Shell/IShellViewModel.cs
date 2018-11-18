using System;

namespace BookCollector.Screens.Shell
{
    public interface IShellViewModel
    {
        void NavigateTo(Type module, bool show_windows_commands = true);

        void ShowMessage(string content);
        void ShowMessage(string content, string action_content, Action action_handler);

        void UpdateSnackbarQueue();
    }
}
