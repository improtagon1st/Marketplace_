using MarketplaceWPF.Models;
using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class ProductDetailsPage : Page
    {
        public ProductDetailsPage(ProductModel product)
        {
            InitializeComponent();
            DataContext = new ProductDetailsViewModel(product);
        }
    }
}