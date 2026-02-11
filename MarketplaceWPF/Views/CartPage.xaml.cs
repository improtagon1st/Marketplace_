using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class CartPage : Page
    {
        public CartPage()
        {
            InitializeComponent();
            DataContext = new CartViewModel();
        }
    }
}