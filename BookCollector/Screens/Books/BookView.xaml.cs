using Panda.Utils;
using System.Windows;
using System.Windows.Controls;

namespace BookCollector.Screens.Books
{
    public partial class BookView
    {
        public BookView()
        {
            InitializeComponent();
        }

        private void TagsLoaded(object sender, RoutedEventArgs e)
        {
            (sender as DataGrid).Columns
                                .Apply(c => c.Width = new DataGridLength(1, DataGridLengthUnitType.Star));
        }
    }
}
