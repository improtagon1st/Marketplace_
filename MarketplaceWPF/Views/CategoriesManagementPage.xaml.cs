using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class CategoriesManagementPage : Page
    {
        public CategoriesManagementPage()
        {
            InitializeComponent();
            DataContext = new CategoriesManagementViewModel();
        }

        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }
    }
}