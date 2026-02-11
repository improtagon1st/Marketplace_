using MarketplaceWPF.Models;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class CategoryEditDialog : Window
    {
        public CategoryModel Category { get; private set; }
        private bool _isEditMode;

        // Конструктор для добавления новой категории
        public CategoryEditDialog()
        {
            InitializeComponent();

            _isEditMode = false;
            TitleText.Text = "Добавить категорию";
        }

        // Конструктор для редактирования существующей категории
        public CategoryEditDialog(CategoryModel category)
        {
            InitializeComponent();

            _isEditMode = true;
            TitleText.Text = "Редактировать категорию";

            // Заполняем поля данными категории
            NameBox.Text = category.Name;
            DescriptionBox.Text = category.Description;

            Category = category;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название категории");
                return;
            }

            // Создаём или обновляем категорию
            if (_isEditMode)
            {
                // Обновляем существующую категорию
                Category.Name = NameBox.Text;
                Category.Description = DescriptionBox.Text;
            }
            else
            {
                // Создаём новую категорию
                Category = new CategoryModel
                {
                    Name = NameBox.Text,
                    Description = DescriptionBox.Text
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