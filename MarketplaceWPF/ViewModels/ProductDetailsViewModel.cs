using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class ProductDetailsViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ProductModel _product;
        private ObservableCollection<PickupPointModel> _pickupPoints;

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

        public ICommand CreateOrderCommand { get; }
        public ICommand BackCommand { get; }

        public ProductDetailsViewModel(int productId)
        {
            _apiService = new ApiService { Token = UserSession.Token };
            PickupPoints = new ObservableCollection<PickupPointModel>();

            CreateOrderCommand = new RelayCommand(async (param) => await CreateOrder());
            BackCommand = new RelayCommand((param) => GoBack());

            LoadData(productId);
        }

        private async void LoadData(int productId)
        {
            await LoadProduct(productId);
            await LoadPickupPoints();
        }

        private async Task LoadProduct(int productId)
        {
            var product = await _apiService.GetAsync<ProductModel>($"Products/{productId}");

            if (product != null)
            {
                Product = product;
            }
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
            }
        }

        private async Task CreateOrder()
        {
            if (Product == null) return;

            var dialog = new Views.OrderDialog(PickupPoints.ToList(), Product);

            if (dialog.ShowDialog() == true)
            {
                var selectedPickupPoint = dialog.SelectedPickupPoint;
                var quantity = dialog.Quantity;

                var orderRequest = new
                {
                    PickupPointId = selectedPickupPoint.Id,
                    Items = new[]
                    {
                        new
                        {
                            ProductId = Product.Id,
                            Quantity = quantity
                        }
                    }
                };

                var response = await _apiService.PostAsync<dynamic>("Orders", orderRequest);

                if (response != null)
                {
                    MessageBox.Show($"Заказ создан!\nКод для получения: {response.qrCode}\nСумма: {response.totalPrice} руб.");
                    await LoadProduct(Product.Id); // Обновляем остаток
                }
                else
                {
                    MessageBox.Show("Ошибка создания заказа");
                }
            }
        }

        private void GoBack()
        {
            // Возврат на предыдущую страницу
            Application.Current.Windows.OfType<Views.MainWindow>()
                .FirstOrDefault()?.ContentFrame.GoBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}