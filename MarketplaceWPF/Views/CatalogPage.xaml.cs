using MarketplaceWPF.Models;
using System.Windows.Controls;
using System.Windows.Input;

namespace MarketplaceWPF.Views
{
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
        }

        private void ProductCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element &&
                element.Tag is ProductModel product)
            {
                NavigationService?.Navigate(new ProductDetailsPage(product.Id));
            }
        }
    }
}