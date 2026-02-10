using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        private ObservableCollection<ProductModel> _products;
        private ObservableCollection<CategoryModel> _categories;
        private ObservableCollection<PickupPointModel> _pickupPoints;
        private CategoryModel _selectedCategory;
        private string _searchText;

        public ObservableCollection<ProductModel> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged(nameof(Products));
            }
        }

        public ObservableCollection<CategoryModel> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
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

        public CategoryModel SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                LoadProducts();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
            }
        }

        public ICommand SearchCommand { get; }
        public ICommand CreateOrderCommand { get; }

        public CatalogViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };

            Products = new ObservableCollection<ProductModel>();
            Categories = new ObservableCollection<CategoryModel>();
            PickupPoints = new ObservableCollection<PickupPointModel>();

            SearchCommand = new RelayCommand(async (param) => await LoadProducts());
            CreateOrderCommand = new RelayCommand(async (param) => await CreateOrder(param));

            LoadData();
        }

        private async void LoadData()
        {
            await LoadCategories();
            await LoadProducts();
            await LoadPickupPoints();
        }

        private async Task LoadCategories()
        {
            var categories = await _apiService.GetAsync<List<CategoryModel>>("Categories");

            if (categories != null)
            {
                Categories.Clear();
                Categories.Add(new CategoryModel { Id = 0, Name = "Все категории" });

                foreach (var category in categories)
                {
                    Categories.Add(category);
                }

                SelectedCategory = Categories[0];
            }
        }

        private async Task LoadProducts()
        {
            var endpoint = "Products";

            if (SelectedCategory != null && SelectedCategory.Id > 0)
            {
                endpoint += $"?categoryId={SelectedCategory.Id}";
            }

            if (!string.IsNullOrEmpty(SearchText))
            {
                var separator = endpoint.Contains("?") ? "&" : "?";
                endpoint += $"{separator}search={SearchText}";
            }

            var products = await _apiService.GetAsync<List<ProductModel>>(endpoint);

            if (products != null)
            {
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
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

        private async Task CreateOrder(object parameter)
        {
            if (parameter is ProductModel product)
            {
                // Показываем диалог выбора ПВЗ
                var dialog = new Views.OrderDialog(PickupPoints.ToList(), product);

                if (dialog.ShowDialog() == true)
                {
                    var selectedPickupPoint = dialog.SelectedPickupPoint;
                    var quantity = dialog.Quantity;

                    // Создаем заказ
                    var orderRequest = new
                    {
                        PickupPointId = selectedPickupPoint.Id,
                        Items = new[]
                        {
                            new
                            {
                                ProductId = product.Id,
                                Quantity = quantity
                            }
                        }
                    };

                    var response = await _apiService.PostAsync<dynamic>("Orders", orderRequest);

                    if (response != null)
                    {
                        MessageBox.Show($"Заказ создан!\nКод для получения: {response.qrCode}\nСумма: {response.totalPrice} руб.");
                        await LoadProducts(); // Обновляем остатки
                    }
                    else
                    {
                        MessageBox.Show("Ошибка создания заказа");
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