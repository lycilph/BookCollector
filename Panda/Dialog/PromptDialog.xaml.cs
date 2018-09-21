using System.Windows;

namespace Panda.Dialog
{
    public partial class PromptDialog
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(InputDialog), new PropertyMetadata(null));

        public PromptDialog()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OkClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
