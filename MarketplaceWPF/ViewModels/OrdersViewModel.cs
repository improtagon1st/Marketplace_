using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace MarketplaceWPF.ViewModels
{
    public class OrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<OrderModel> _orders;
        private OrderModel _selectedOrder;

        public ObservableCollection<OrderModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public OrderModel SelectedOrder
        {
            get => _selectedOrder;
            set
            {
                _selectedOrder = value;
                OnPropertyChanged(nameof(SelectedOrder));
            }
        }

        public ICommand ShowQRCommand { get; }

        public OrdersViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Orders = new ObservableCollection<OrderModel>();

            ShowQRCommand = new RelayCommand((param) => ShowQR(param));

            LoadOrders();
        }

        private async void LoadOrders()
        {
            var orders = await _apiService.GetAsync<List<OrderModel>>("Orders");

            if (orders != null)
            {
                Orders.Clear();
                foreach (var order in orders)
                {
                    Orders.Add(order);
                }
            }
        }

        private void ShowQR(object parameter)
        {
            if (parameter is OrderModel order)
            {
                // Открываем окно с QR-кодом
                var qrWindow = new Views.QRCodeWindow(order);
                qrWindow.ShowDialog();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}