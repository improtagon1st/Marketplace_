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
        private string _qrCode;
        private OrderModel _foundOrder;
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

        public OrderModel FoundOrder
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

            SearchOrderCommand = new RelayCommand(async (param) => await SearchOrder());
            PickupOrderCommand = new RelayCommand(async (param) => await PickupOrder());
            ClearCommand = new RelayCommand((param) => Clear());
        }

        private async Task SearchOrder()
        {
            if (string.IsNullOrEmpty(QRCode))
            {
                MessageBox.Show("Введите код заказа");
                return;
            }

            var order = await _apiService.GetAsync<OrderModel>($"Orders/bycode/{QRCode}");

            if (order != null)
            {
               
                if (UserSession.IsWorker && UserSession.PickupPointId.HasValue)
                {
                    if (order.PickupPointId != UserSession.PickupPointId.Value)
                    {
                        System.Media.SystemSounds.Hand.Play(); // Звук ошибки
                        MessageBox.Show(
                            $"❌ ОШИБКА!\n\n" +
                            $"Этот заказ предназначен для другого ПВЗ!\n\n" +
                            $"Заказ должен быть выдан в:\n{order.PickupPointName}\n{order.PickupPointAddress}",
                            "Неверный пункт выдачи",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);

                        Clear();
                        return;
                    }
                }

                FoundOrder = order;
                IsOrderFound = true;
                System.Media.SystemSounds.Asterisk.Play(); // Звук успеха
            }
            else
            {
                System.Media.SystemSounds.Hand.Play(); // Звук ошибки
                MessageBox.Show("Заказ с таким кодом не найден");
                IsOrderFound = false;
                FoundOrder = null;
            }
        }

        private async Task PickupOrder()
        {
            if (FoundOrder == null) return;

            if (FoundOrder.Status == "PickedUp")
            {
                MessageBox.Show("Этот заказ уже был выдан ранее");
                return;
            }

            var result = MessageBox.Show(
                $"Выдать заказ #{FoundOrder.Id} клиенту {FoundOrder.CustomerName}?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                var success = await _apiService.PutAsync($"Orders/{FoundOrder.Id}/pickup");

                if (success)
                {
                    MessageBox.Show("Заказ успешно выдан!");
                    Clear();
                }
                else
                {
                    MessageBox.Show("Ошибка при выдаче заказа");
                }
            }
        }

        private void Clear()
        {
            QRCode = string.Empty;
            FoundOrder = null;
            IsOrderFound = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}