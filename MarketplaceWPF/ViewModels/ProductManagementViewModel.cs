using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;


namespace MarketplaceWPF.ViewModels
{
    public class ProductManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<ProductModel> _products;
        private ObservableCollection<CategoryModel> _categories;

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

        public ICommand AddProductCommand { get; }
        public ICommand EditProductCommand { get; }
        public ICommand DeleteProductCommand { get; }
        public ICommand RefreshCommand { get; }

        public ProductManagementViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Products = new ObservableCollection<ProductModel>();
            Categories = new ObservableCollection<CategoryModel>();

            AddProductCommand = new RelayCommand((param) => AddProduct());
            EditProductCommand = new RelayCommand((param) => EditProduct(param));
            DeleteProductCommand = new RelayCommand(async (param) => await DeleteProduct(param));
            RefreshCommand = new RelayCommand(async (param) => await LoadData());

            LoadData();
        }

        private async Task LoadData()
        {
            await LoadCategories();
            await LoadProducts();
        }

        private async Task LoadCategories()
        {
            var categories = await _apiService.GetAsync<List<CategoryModel>>("Categories");

            if (categories != null)
            {
                Categories.Clear();
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
        }

        private async Task LoadProducts()
        {
            var products = await _apiService.GetAsync<List<ProductModel>>("Products");

            if (products != null)
            {
                Products.Clear();
                foreach (var product in products)
                {
                    Products.Add(product);
                }
            }
        }

        private void AddProduct()
        {
            var dialog = new Views.ProductEditDialog(Categories.ToList());

            if (dialog.ShowDialog() == true)
            {
                var product = dialog.Product;
                CreateProduct(product);
            }
        }

        private void EditProduct(object parameter)
        {
            if (parameter is ProductModel product)
            {
                var dialog = new Views.ProductEditDialog(Categories.ToList(), product);

                if (dialog.ShowDialog() == true)
                {
                    var updatedProduct = dialog.Product;
                    UpdateProduct(updatedProduct);
                }
            }
        }

        private async void CreateProduct(ProductModel product)
        {
            var dto = new ProductCreateDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };

        

            var response = await _apiService.PostAsync<ProductModel>("Products", dto);

            if (response != null)
            {
                MessageBox.Show("Товар успешно добавлен!");
                await LoadProducts();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении товара");
            }
        }

        private async void UpdateProduct(ProductModel product)
        {
            var dto = new ProductCreateDto
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                ImageUrl = product.ImageUrl
            };

            var success = await _apiService.PutAsync($"Products/{product.Id}", dto);

            if (success)
            {
                MessageBox.Show("Товар успешно обновлён!");
                await LoadProducts();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении товара");
            }
        }

        private async Task DeleteProduct(object parameter)
        {
            if (parameter is ProductModel product)
            {
                var result = MessageBox.Show(
                    $"Удалить товар '{product.Name}'?",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var response = await _apiService.DeleteAsync($"Products/{product.Id}");

                    if (response)
                    {
                        MessageBox.Show("Товар удалён!");
                        await LoadProducts();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении товара");
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