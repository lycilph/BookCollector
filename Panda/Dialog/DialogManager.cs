using Microsoft.Win32;
using System;
using System.Windows;

namespace Panda.Dialog
{
    public class DialogManager : IDialogManager
    {
        public DialogResult ShowPromtDialog(string caption, string message)
        {
            var prompt_dialog = new PromptDialog
            {
                Owner = GetWindow(),
                Title = caption,
                Message = message
            };
            var result = prompt_dialog.ShowDialog();

            return new DialogResult(result);
        }

        public InputDialogResult ShowInputDialog(string caption, string message, bool can_cancel = true)
        {
            var input_dialog = new InputDialog
            {
                Owner = GetWindow(),
                Title = caption,
                Message = message,
                CanCancel = can_cancel
            };
            var result = input_dialog.ShowDialog();

            return new InputDialogResult(result, input_dialog.Input);
        }

        public FileDialogResult ShowOpenFileDialog(string title, string extension, string filter)
        {
            var open_file_dialog = new OpenFileDialog
            {
                Title = title,
                DefaultExt = extension,
                Filter = filter
            };
            var result = open_file_dialog.ShowDialog(GetWindow());

            return new FileDialogResult(result, open_file_dialog.FileName, open_file_dialog.SafeFileName);
        }

        public FileDialogResult ShowSafeFileDialog(string title, string extension, string filter)
        {
            var safe_file_dialog = new SaveFileDialog
            {
                Title = title,
                DefaultExt = extension,
                Filter = filter
            };
            var result = safe_file_dialog.ShowDialog(GetWindow());

            return new FileDialogResult(result, safe_file_dialog.FileName, safe_file_dialog.SafeFileName);
        }

        private Window GetWindow()
        {
            if (!(System.Windows.Application.Current.MainWindow is Window window))
                throw new InvalidOperationException("Main window must be a Window");
            return window;
        }
    }
}
