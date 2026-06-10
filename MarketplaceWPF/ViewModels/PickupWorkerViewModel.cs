using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class PickupWorkerViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private string _qrCode = string.Empty;
        private OrderModel? _foundOrder;
        private bool _isOrderFound;

        public string QRCode
        {
            get => _qrCode;
            set
            {
                _qrCode = value;
                OnPropertyChanged(nameof(QRCode));
            }
        }

        public OrderModel? FoundOrder
        {
            get => _foundOrder;
            set
            {
                _foundOrder = value;
                OnPropertyChanged(nameof(FoundOrder));
            }
        }

        public bool IsOrderFound
        {
            get => _isOrderFound;
            set
            {
                _isOrderFound = value;
                OnPropertyChanged(nameof(IsOrderFound));
            }
        }

        public ICommand SearchOrderCommand { get; }
        public ICommand PickupOrderCommand { get; }
        public ICommand ClearCommand { get; }

        public PickupWorkerViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };

            SearchOrderCommand = new RelayCommand(async _ => await SearchOrder());
            PickupOrderCommand = new RelayCommand(async _ => await PickupOrder());
            ClearCommand = new RelayCommand(_ => Clear());
        }

        private async Task SearchOrder()
        {
            if (string.IsNullOrWhiteSpace(QRCode))
            {
                MessageBox.Show("Введите код заказа");
                return;
            }

            var order = await _apiService.GetAsync<OrderModel>($"Orders/bycode/{QRCode}");

            if (order == null)
            {
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show("Заказ с таким кодом не найден");
                IsOrderFound = false;
                FoundOrder = null;
                return;
            }

            if (UserSession.IsWorker && UserSession.PickupPointId.HasValue && order.PickupPointId != UserSession.PickupPointId.Value)
            {
                System.Media.SystemSounds.Hand.Play();
                MessageBox.Show(
                    $"Этот заказ предназначен для другого ПВЗ.\n\n" +
                    $"Заказ должен быть выдан в:\n{order.PickupPointName}\n{order.PickupPointAddress}",
                    "Неверный пункт выдачи",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Clear();
                return;
            }

            FoundOrder = order;
            IsOrderFound = true;
            System.Media.SystemSounds.Asterisk.Play();
        }

        private async Task PickupOrder()
        {
            if (FoundOrder == null)
            {
                return;
            }

            if (FoundOrder.Status == "PickedUp")
            {
                MessageBox.Show("Этот заказ уже был выдан ранее");
                return;
            }

            if (FoundOrder.Status != "Delivered")
            {
                MessageBox.Show("Заказ нельзя выдать, пока он не доставлен в выбранный ПВЗ");
                return;
            }

            var result = MessageBox.Show(
                $"Выдать заказ #{FoundOrder.Id} клиенту {FoundOrder.CustomerName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            var success = await _apiService.PutAsync($"Orders/{FoundOrder.Id}/pickup");

            if (success)
            {
                MessageBox.Show("Заказ успешно выдан");
                Clear();
            }
            else
            {
                MessageBox.Show("Не удалось выдать заказ");
            }
        }

        private void Clear()
        {
            QRCode = string.Empty;
            FoundOrder = null;
            IsOrderFound = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
