using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class ProductManagementPage : Page
    {
        public ProductManagementPage()
        {
            InitializeComponent();
            DataContext = new ProductManagementViewModel();
        }
    }
}