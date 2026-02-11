using MarketplaceWPF.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

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

            // ОТЛАДКА - проверим что выбрано
            int categoryId = (int)CategoryCombo.SelectedValue;
            MessageBox.Show($"Selected CategoryId: {categoryId}");

            // Создаём или обновляем товар
            if (_isEditMode)
            {
                // Обновляем существующий товар
                Product.Name = NameBox.Text;
                Product.Description = DescriptionBox.Text;
                Product.Price = price;
                Product.Stock = stock;
                Product.CategoryId = categoryId;
                Product.ImageUrl = ImageUrlBox.Text;
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
                    ImageUrl = ImageUrlBox.Text
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
    }
}