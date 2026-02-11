using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class PickupPointOrdersViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<OrderModel> _orders;
        private string _selectedFilter;

        public ObservableCollection<OrderModel> Orders
        {
            get => _orders;
            set
            {
                _orders = value;
                OnPropertyChanged(nameof(Orders));
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
        public ICommand MarkDeliveredCommand { get; }

        public PickupPointOrdersViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Orders = new ObservableCollection<OrderModel>();
            SelectedFilter = "All";

            RefreshCommand = new RelayCommand(async (param) => await LoadOrders());
            MarkDeliveredCommand = new RelayCommand(async (param) => await MarkDelivered(param));

            LoadOrders();
        }

        private async Task LoadOrders()
        {
            var endpoint = "Orders";

            // Фильтр по статусу
            if (SelectedFilter == "Created")
            {
                endpoint += "?status=Created";
            }
            else if (SelectedFilter == "Delivered")
            {
                endpoint += "?status=Delivered";
            }
            else if (SelectedFilter == "PickedUp")
            {
                endpoint += "?status=PickedUp";
            }

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

        private async Task MarkDelivered(object parameter)
        {
            if (parameter is OrderModel order)
            {
                if (order.Status != "Created")
                {
                    MessageBox.Show("Этот заказ уже обработан");
                    return;
                }

                var result = MessageBox.Show(
                    $"Отметить заказ #{order.Id} как доставленный в ПВЗ?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _apiService.PutAsync($"Orders/{order.Id}/deliver");

                    if (success)
                    {
                        MessageBox.Show("Заказ отмечен как доставленный!");
                        await LoadOrders();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при обновлении статуса");
                    }
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}