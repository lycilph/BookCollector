using System.Windows;

namespace Panda.Dialog
{
    public partial class InputDialog
    {
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(InputDialog), new PropertyMetadata(null));

        public bool CanCancel
        {
            get { return (bool)GetValue(CanCancelProperty); }
            set { SetValue(CanCancelProperty, value); }
        }
        public static readonly DependencyProperty CanCancelProperty =
            DependencyProperty.Register("CanCancel", typeof(bool), typeof(InputDialog), new PropertyMetadata(true));

        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }
        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register("Input", typeof(string), typeof(InputDialog), new PropertyMetadata(null));

        public InputDialog()
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
