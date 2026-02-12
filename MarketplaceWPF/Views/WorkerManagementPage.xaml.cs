using MarketplaceWPF.ViewModels;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class WorkerManagementPage : Page
    {
        public WorkerManagementPage()
        {
            InitializeComponent();
            DataContext = new WorkerManagementViewModel();
        }
    }
}