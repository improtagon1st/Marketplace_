using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class PickupPointOrdersPage : Page
    {
        private PickupPointOrdersViewModel _viewModel;

        public PickupPointOrdersPage()
        {
            InitializeComponent();
            _viewModel = new PickupPointOrdersViewModel();
            DataContext = _viewModel;
        }

        private void Filter_Changed(object sender, System.Windows.RoutedEventArgs e)
        {
            if (sender is RadioButton rb && _viewModel != null)
            {
                _viewModel.SelectedFilter = rb.Tag?.ToString() ?? "All";
            }
        }
    }
}