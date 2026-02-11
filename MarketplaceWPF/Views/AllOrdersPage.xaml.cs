using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class AllOrdersPage : Page
    {
        private AllOrdersViewModel _viewModel;

        public AllOrdersPage()
        {
            InitializeComponent();
            _viewModel = new AllOrdersViewModel();
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