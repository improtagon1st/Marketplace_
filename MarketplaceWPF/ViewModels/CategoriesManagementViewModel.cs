using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class CategoriesManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<CategoryModel> _categories;

        public ObservableCollection<CategoryModel> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged(nameof(Categories));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public CategoriesManagementViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Categories = new ObservableCollection<CategoryModel>();

            AddCommand = new RelayCommand((param) => Add());
            EditCommand = new RelayCommand((param) => Edit(param));
            DeleteCommand = new RelayCommand(async (param) => await Delete(param));
            RefreshCommand = new RelayCommand(async (param) => await LoadData());

            LoadData();
        }

        private async Task LoadData()
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

        private void Add()
        {
            var dialog = new Views.CategoryEditDialog();

            if (dialog.ShowDialog() == true)
            {
                var category = dialog.Category;
                Create(category);
            }
        }

        private void Edit(object parameter)
        {
            if (parameter is CategoryModel category)
            {
                var dialog = new Views.CategoryEditDialog(category);

                if (dialog.ShowDialog() == true)
                {
                    var updated = dialog.Category;
                    Update(updated);
                }
            }
        }

        private async void Create(CategoryModel category)
        {
            var response = await _apiService.PostAsync<CategoryModel>("Categories", category);

            if (response != null)
            {
                MessageBox.Show("Категория успешно добавлена!");
                await LoadData();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении категории");
            }
        }

        private async void Update(CategoryModel category)
        {
            var success = await _apiService.PutAsync($"Categories/{category.Id}", category);

            if (success)
            {
                MessageBox.Show("Категория успешно обновлена!");
                await LoadData();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении категории");
            }
        }

        private async Task Delete(object parameter)
        {
            if (parameter is CategoryModel category)
            {
                var result = MessageBox.Show(
                    $"Удалить категорию '{category.Name}'?\n\nВнимание: Это может повлиять на товары в этой категории!",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _apiService.DeleteAsync($"Categories/{category.Id}");

                    if (success)
                    {
                        MessageBox.Show("Категория удалена!");
                        await LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении категории");
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