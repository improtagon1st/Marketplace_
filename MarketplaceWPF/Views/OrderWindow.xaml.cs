using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class OrderWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly ProductModel _product;

        public OrderWindow(ProductModel product)
        {
            InitializeComponent();

            _apiService = new ApiService { Token = UserSession.Token };
            _product = product;

            ProductNameText.Text = product.Name;
            ProductPriceText.Text = $"{product.Price:F2} руб.";

            LoadPickupPoints();
        }

        private async void LoadPickupPoints()
        {
            var pickupPoints = await _apiService.GetAsync<List<PickupPointModel>>("PickupPoints");

            if (pickupPoints != null && pickupPoints.Any())
            {
                PickupPointCombo.ItemsSource = pickupPoints;
                PickupPointCombo.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("Не удалось загрузить пункты выдачи");
            }
        }

        private async void Order_Click(object sender, RoutedEventArgs e)
        {
            if (PickupPointCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи");
                return;
            }

            var selectedPickupPoint = (PickupPointModel)PickupPointCombo.SelectedItem;

            var request = new CreateOrderRequest
            {
                PickupPointId = selectedPickupPoint.Id,
                Items = new List<OrderItemRequest>
        {
            new OrderItemRequest
            {
                ProductId = _product.Id,
                Quantity = 1
            }
        }
            };

            var response = await _apiService.PostAsync<CreateOrderResponse>("Orders", request);

            if (response != null)
            {
                // Загружаем полные данные заказа
                var fullOrder = await _apiService.GetAsync<OrderModel>($"Orders/{response.OrderId}");

                if (fullOrder != null)
                {
                    MessageBox.Show($"✅ Заказ #{response.OrderId} успешно создан!\nQR-код: {response.QRCode}");

                    // Показываем QR-код с полными данными
                    var qrWindow = new QRCodeWindow(fullOrder);
                    qrWindow.ShowDialog();

                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Заказ создан, но не удалось загрузить детали");
                }
            }
            else
            {
                MessageBox.Show("Ошибка при создании заказа");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}