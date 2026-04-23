using MarketplaceWPF.Models;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace MarketplaceWPF.Views
{
    public partial class ProductEditDialog : Window
    {
        public ProductModel Product { get; private set; }
        private bool _isEditMode;

        // Конструктор для добавления нового товара
        public ProductEditDialog(List<CategoryModel> categories)
        {
            InitializeComponent();

            _isEditMode = false;
            TitleText.Text = "Добавить товар";

            CategoryCombo.ItemsSource = categories;
            if (categories.Any())
            {
                CategoryCombo.SelectedIndex = 0;
            }
        }

        // Конструктор для редактирования существующего товара
        public ProductEditDialog(List<CategoryModel> categories, ProductModel product)
        {
            InitializeComponent();

            _isEditMode = true;
            TitleText.Text = "Редактировать товар";

            CategoryCombo.ItemsSource = categories;

            // Заполняем поля данными товара
            NameBox.Text = product.Name;
            DescriptionBox.Text = product.Description;
            PriceBox.Text = product.Price.ToString();
            StockBox.Text = product.Stock.ToString();
            ImageUrlBox.Text = product.ImageUrl;
            UpdateImagePreview();

            // Выбираем категорию
            CategoryCombo.SelectedValue = product.CategoryId;

            Product = product;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название товара");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price) || price <= 0)
            {
                MessageBox.Show("Введите корректную цену");
                return;
            }

            if (!int.TryParse(StockBox.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            if (CategoryCombo.SelectedValue == null)
            {
                MessageBox.Show("Выберите категорию");
                return;
            }

            int categoryId = (int)CategoryCombo.SelectedValue;
            string imageUrl = ImageUrlBox.Text.Trim();

            // Создаём или обновляем товар
            if (_isEditMode)
            {
                // Обновляем существующий товар
                Product.Name = NameBox.Text;
                Product.Description = DescriptionBox.Text;
                Product.Price = price;
                Product.Stock = stock;
                Product.CategoryId = categoryId;
                Product.ImageUrl = imageUrl;
            }
            else
            {
                // Создаём новый товар
                Product = new ProductModel
                {
                    Name = NameBox.Text,
                    Description = DescriptionBox.Text,
                    Price = price,
                    Stock = stock,
                    CategoryId = categoryId,
                    ImageUrl = imageUrl
                };
            }

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ImageUrlBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateImagePreview();
        }

        private void ImagePreview_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            ShowImagePlaceholder("Не удалось загрузить изображение. Проверьте ссылку.");
        }

        private void UpdateImagePreview()
        {
            if (ImagePreview == null || ImagePreviewPlaceholder == null || ImageUrlBox == null)
            {
                return;
            }

            string imageUrl = ImageUrlBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(imageUrl))
            {
                ShowImagePlaceholder("Предпросмотр появится после ввода прямой ссылки на изображение");
                return;
            }

            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                ShowImagePlaceholder("Введите прямую ссылку на изображение, начинающуюся с http или https");
                return;
            }

            try
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
                bitmap.UriSource = uri;
                bitmap.EndInit();

                ImagePreview.Source = bitmap;
                ImagePreview.Visibility = Visibility.Visible;
                ImagePreviewPlaceholder.Visibility = Visibility.Collapsed;
            }
            catch
            {
                ShowImagePlaceholder("Не удалось загрузить изображение. Проверьте ссылку.");
            }
        }

        private void ShowImagePlaceholder(string message)
        {
            ImagePreview.Source = null;
            ImagePreview.Visibility = Visibility.Collapsed;
            ImagePreviewPlaceholder.Text = message;
            ImagePreviewPlaceholder.Visibility = Visibility.Visible;
        }
    }
}
