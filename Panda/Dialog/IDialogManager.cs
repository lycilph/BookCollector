namespace Panda.Dialog
{
    public interface IDialogManager
    {
        DialogResult ShowPromtDialog(string caption, string message);
        InputDialogResult ShowInputDialog(string caption, string message, bool can_cancel = true);
        FileDialogResult ShowOpenFileDialog(string title, string extension, string filter);
        FileDialogResult ShowSafeFileDialog(string title, string extension, string filter);
    }
}