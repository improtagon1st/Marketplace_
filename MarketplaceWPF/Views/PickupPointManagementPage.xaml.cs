using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class PickupPointManagementPage : Page
    {
        public PickupPointManagementPage()
        {
            InitializeComponent();
            DataContext = new PickupPointManagementViewModel();
        }
    }
}