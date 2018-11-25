using System.Windows.Input;

namespace BookCollector.Screens.Books
{
    public partial class ShelvesView
    {
        public ShelvesView()
        {
            InitializeComponent();
        }

        private void DisregardMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
