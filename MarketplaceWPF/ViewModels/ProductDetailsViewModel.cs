using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System;
using System.Net.Http;

namespace MarketplaceWPF.ViewModels
{
    public class ProductDetailsViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ProductModel _product;
        private ObservableCollection<PickupPointModel> _pickupPoints;
        private PickupPointModel _selectedPickupPoint;

        public ProductModel Product
        {
            get => _product;
            set
            {
                _product = value;
                OnPropertyChanged(nameof(Product));
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
            }
        }

        public ICommand CreateOrderCommand { get; }
        public ICommand BackCommand { get; }
        public ICommand BuyNowCommand { get; }
        public ICommand AddToCartCommand { get; }

        public ProductDetailsViewModel(ProductModel product)
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Product = product;
            PickupPoints = new ObservableCollection<PickupPointModel>();

            CreateOrderCommand = new RelayCommand(async (param) => await CreateOrder());
            BackCommand = new RelayCommand((param) => GoBack());
            BuyNowCommand = new RelayCommand((param) => BuyNow());
            AddToCartCommand = new RelayCommand(async (param) => await AddToCart());

            LoadPickupPoints();
        }

        private async Task LoadPickupPoints()
        {
            var pickupPoints = await _apiService.GetAsync<List<PickupPointModel>>("PickupPoints");

            if (pickupPoints != null)
            {
                PickupPoints.Clear();
                foreach (var pp in pickupPoints)
                {
                    PickupPoints.Add(pp);
                }

                if (PickupPoints.Count > 0)
                {
                    SelectedPickupPoint = PickupPoints[0];
                }
            }
        }

        private void BuyNow()
        {
            if (Product.Stock <= 0)
            {
                MessageBox.Show("Товар закончился на складе");
                return;
            }

            var orderWindow = new Views.OrderWindow(Product);
            orderWindow.ShowDialog();
        }
        private async Task CreateOrder()
        {
            if (Product == null) return;

            var dialog = new Views.OrderDialog(PickupPoints.ToList(), Product);

            if (dialog.ShowDialog() == true)
            {
                var selectedPickupPoint = dialog.SelectedPickupPoint;
                var quantity = dialog.Quantity;

                var orderRequest = new CreateOrderRequest
                {
                    PickupPointId = selectedPickupPoint.Id,
                    Items = new List<OrderItemRequest>
                    {
                        new OrderItemRequest
                        {
                            ProductId = Product.Id,
                            Quantity = quantity
                        }
                    }
                };

                var response = await _apiService.PostAsync<CreateOrderResponse>("Orders", orderRequest);

                if (response != null)
                {
                    MessageBox.Show($"✅ Заказ #{response.OrderId} успешно создан!\nQR-код: {response.QRCode}");

                    // Показываем QR-код
                    var order = new OrderModel
                    {
                        Id = response.OrderId,
                        QRCode = response.QRCode
                    };

                    var qrWindow = new Views.QRCodeWindow(order);
                    qrWindow.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ошибка при создании заказа");
                }
            }
        }

        private void GoBack()
        {
            // Возврат на предыдущую страницу
            Application.Current.Windows.OfType<Views.MainWindow>()
                .FirstOrDefault()?.ContentFrame.GoBack();
        }

        private async Task AddToCart()
        {
            if (Product.Stock <= 0)
            {
                MessageBox.Show("Товар закончился на складе");
                return;
            }

            var request = new AddToCartRequest
            {
                ProductId = Product.Id,
                Quantity = 1
            };

            try
            {
                // Просто делаем POST запрос без ожидания JSON ответа
                var httpClient = new System.Net.Http.HttpClient();
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                var json = System.Text.Json.JsonSerializer.Serialize(request);
                var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync("https://localhost:7093/api/Cart", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"✅ '{Product.Name}' добавлен в корзину!");
                }
                else
                {
                    MessageBox.Show("Ошибка при добавлении в корзину");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}