using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Net.Http;

namespace MarketplaceWPF.ViewModels
{
    public class CartViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<CartItemModel> _cartItems;
        private decimal _totalAmount;
        private bool _isCheckoutVisible;

        public ObservableCollection<CartItemModel> CartItems
        {
            get => _cartItems;
            set
            {
                _cartItems = value;
                OnPropertyChanged(nameof(CartItems));
                UpdateTotalAmount();
            }
        }

        public decimal TotalAmount
        {
            get => _totalAmount;
            set
            {
                _totalAmount = value;
                OnPropertyChanged(nameof(TotalAmount));
            }
        }

        public bool IsCheckoutVisible
        {
            get => _isCheckoutVisible;
            set
            {
                _isCheckoutVisible = value;
                OnPropertyChanged(nameof(IsCheckoutVisible));
            }
        }

        public ICommand IncreaseQuantityCommand { get; }
        public ICommand DecreaseQuantityCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand CheckoutCommand { get; }
        public ICommand RefreshCommand { get; }

        public CartViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            CartItems = new ObservableCollection<CartItemModel>();

            IncreaseQuantityCommand = new RelayCommand(async (param) => await ChangeQuantity(param, 1));
            DecreaseQuantityCommand = new RelayCommand(async (param) => await ChangeQuantity(param, -1));
            RemoveItemCommand = new RelayCommand(async (param) => await RemoveItem(param));
            CheckoutCommand = new RelayCommand((param) => ShowCheckout());
            RefreshCommand = new RelayCommand(async (param) => await LoadCart());

            LoadCart();
        }

        private async Task LoadCart()
        {
            var items = await _apiService.GetAsync<List<CartItemModel>>("Cart");

            if (items != null)
            {
                CartItems.Clear();
                foreach (var item in items)
                {
                    CartItems.Add(item);
                }

                IsCheckoutVisible = CartItems.Count > 0;
            }
            else
            {
                CartItems.Clear();
                IsCheckoutVisible = false;
            }

            UpdateTotalAmount();
        }

        private async Task ChangeQuantity(object parameter, int change)
        {
            if (parameter is CartItemModel item)
            {
                int newQuantity = item.Quantity + change;

                if (newQuantity <= 0)
                {
                    await RemoveItem(item);
                    return;
                }

                if (newQuantity > item.AvailableStock)
                {
                    MessageBox.Show($"Максимальное доступное количество: {item.AvailableStock}");
                    return;
                }

                var request = new UpdateCartItemRequest { Quantity = newQuantity };

                try
                {
                    var httpClient = new System.Net.Http.HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                    var json = System.Text.Json.JsonSerializer.Serialize(request);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PutAsync($"https://localhost:7093/api/Cart/{item.Id}", content);

                    if (response.IsSuccessStatusCode)
                    {
                        // Обновляем данные элемента
                        item.Quantity = newQuantity;
                        item.TotalPrice = item.ProductPrice * newQuantity;

                        // Перезагружаем корзину чтобы UI обновился
                        await LoadCart();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
        private async Task RemoveItem(object parameter)
        {
            if (parameter is CartItemModel item)
            {
                var result = MessageBox.Show(
                    $"Удалить '{item.ProductName}' из корзины?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _apiService.DeleteAsync($"Cart/{item.Id}");

                    if (success)
                    {
                        CartItems.Remove(item);
                        UpdateTotalAmount();
                        IsCheckoutVisible = CartItems.Count > 0;

                        if (CartItems.Count == 0)
                        {
                            MessageBox.Show("Корзина пуста");
                        }
                    }
                }
            }
        }

        private void ShowCheckout()
        {
            if (CartItems.Count == 0)
            {
                MessageBox.Show("Корзина пуста");
                return;
            }

            var checkoutWindow = new Views.CheckoutWindow(CartItems.ToList(), TotalAmount);
            if (checkoutWindow.ShowDialog() == true)
            {
                // Заказ оформлен, перезагружаем корзину
                LoadCart();
            }
        }

        private void UpdateTotalAmount()
        {
            TotalAmount = CartItems.Sum(item => item.TotalPrice);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}