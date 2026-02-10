using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class ProductDetailsPage : Page
    {
        public ProductDetailsPage(int productId)
        {
            InitializeComponent();
            DataContext = new ProductDetailsViewModel(productId);
        }
    }
}