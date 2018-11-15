using BookCollector.Screens.Dialogs;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using Panda.Infrastructure;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace BookCollector.Application
{
    public static class DialogManager
    {
        public static Task<object> ShowMessageDialogAsync(string title, string message)
        {
            var vm = new MessageDialogViewModel
            {
                Title = title,
                Message = message
            };
            var v = ViewManager.CreateAndBindViewForModel(vm);

            return DialogHost.Show(v);
        }

        public static Task<object> ShowPromptDialogAsync(string title, string message)
        {
            var vm = new PromptDialogViewModel
            {
                Title = title,
                Message = message
            };
            var v = ViewManager.CreateAndBindViewForModel(vm);

            return DialogHost.Show(v);
        }

        public static (bool?, string) ShowSaveFileDialog(string title, string extension, string filter)
        {
            var save_file_dialog = new SaveFileDialog
            {
                Title = title,
                DefaultExt = extension,
                Filter = filter
            };
            var result = save_file_dialog.ShowDialog(GetWindow());

            return (result, save_file_dialog.FileName);
        }

        private static Window GetWindow()
        {
            if (!(System.Windows.Application.Current.MainWindow is Window window))
                throw new InvalidOperationException("Main window must be a Window");
            return window;
        }
    }
}
