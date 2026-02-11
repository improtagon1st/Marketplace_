using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class CheckoutWindow : Window
    {
        private readonly ApiService _apiService;
        private readonly List<CartItemModel> _cartItems;
        private readonly decimal _totalAmount;

        public CheckoutWindow(List<CartItemModel> cartItems, decimal totalAmount)
        {
            InitializeComponent();

            _apiService = new ApiService { Token = Helpers.UserSession.Token };
            _cartItems = cartItems;
            _totalAmount = totalAmount;

            ItemsList.ItemsSource = _cartItems;
            TotalAmountText.Text = $"{_totalAmount:F2} руб.";

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

        private async void Checkout_Click(object sender, RoutedEventArgs e)
        {
            if (PickupPointCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи");
                return;
            }

            var selectedPickupPoint = (PickupPointModel)PickupPointCombo.SelectedItem;

            var request = new CheckoutRequest
            {
                PickupPointId = selectedPickupPoint.Id
            };

            var response = await _apiService.PostAsync<CheckoutResponse>("Cart/checkout", request);

            if (response != null)
            {
                MessageBox.Show($"✅ {response.Message}\n\nНомер заказа: {response.OrderId}\nQR-код: {response.QRCode}");
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Ошибка при оформлении заказа");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}