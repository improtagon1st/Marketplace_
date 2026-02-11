using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class AllOrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<OrderModel> _orders;
        private string _selectedFilter;
        private ObservableCollection<PickupPointModel> _pickupPoints;
        private PickupPointModel _selectedPickupPoint;

        public ObservableCollection<OrderModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
            }
        }

        public ObservableCollection<PickupPointModel> PickupPoints
        {
            get => _pickupPoints;
            set
            {
                _pickupPoints = value;
                OnPropertyChanged(nameof(PickupPoints));
            }
        }

        public PickupPointModel SelectedPickupPoint
        {
            get => _selectedPickupPoint;
            set
            {
                _selectedPickupPoint = value;
                OnPropertyChanged(nameof(SelectedPickupPoint));
                LoadOrders();
            }
        }

        public string SelectedFilter
        {
            get => _selectedFilter;
            set
            {
                _selectedFilter = value;
                OnPropertyChanged(nameof(SelectedFilter));
                LoadOrders();
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand ShowQRCommand { get; }

        public AllOrdersViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Orders = new ObservableCollection<OrderModel>();
            PickupPoints = new ObservableCollection<PickupPointModel>();
            SelectedFilter = "All";

            RefreshCommand = new RelayCommand(async (param) => await LoadOrders());
            ShowQRCommand = new RelayCommand((param) => ShowQR(param));

            LoadPickupPoints();
            LoadOrders();
        }

        private async void LoadPickupPoints()
        {
            var pickupPoints = await _apiService.GetAsync<List<PickupPointModel>>("PickupPoints");

            if (pickupPoints != null)
            {
                PickupPoints.Clear();
                PickupPoints.Add(new PickupPointModel { Id = 0, Name = "Все ПВЗ" });

                foreach (var pp in pickupPoints)
                {
                    PickupPoints.Add(pp);
                }

                SelectedPickupPoint = PickupPoints[0];
            }
        }

        private async Task LoadOrders()
        {
            var endpoint = "Orders?";

            // Фильтр по ПВЗ
            if (SelectedPickupPoint != null && SelectedPickupPoint.Id > 0)
            {
                endpoint += $"pickupPointId={SelectedPickupPoint.Id}&";
            }

            // Фильтр по статусу
            if (SelectedFilter != "All")
            {
                endpoint += $"status={SelectedFilter}&";
            }

            endpoint = endpoint.TrimEnd('&', '?');

            var orders = await _apiService.GetAsync<List<OrderModel>>(endpoint);

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