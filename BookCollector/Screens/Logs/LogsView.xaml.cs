using System.Windows;

namespace BookCollector.Screens.Logs
{
    public partial class LogsView
    {
        public LogsView()
        {
            InitializeComponent();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            logs_list.ScrollToBottom();
        }
    }
}
