using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class PickupPointManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<PickupPointModel> _pickupPoints;

        public ObservableCollection<PickupPointModel> PickupPoints
        {
            get => _pickupPoints;
            set
            {
                _pickupPoints = value;
                OnPropertyChanged(nameof(PickupPoints));
            }
        }

        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand RefreshCommand { get; }

        public PickupPointManagementViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            PickupPoints = new ObservableCollection<PickupPointModel>();

            AddCommand = new RelayCommand((param) => Add());
            EditCommand = new RelayCommand((param) => Edit(param));
            DeleteCommand = new RelayCommand(async (param) => await Delete(param));
            RefreshCommand = new RelayCommand(async (param) => await LoadData());

            LoadData();
        }

        private async Task LoadData()
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

        private void Add()
        {
            var dialog = new Views.PickupPointEditDialog();

            if (dialog.ShowDialog() == true)
            {
                var pickupPoint = dialog.PickupPoint;
                Create(pickupPoint);
            }
        }

        private void Edit(object parameter)
        {
            if (parameter is PickupPointModel pp)
            {
                var dialog = new Views.PickupPointEditDialog(pp);

                if (dialog.ShowDialog() == true)
                {
                    var updated = dialog.PickupPoint;
                    Update(updated);
                }
            }
        }

        private async void Create(PickupPointModel pickupPoint)
        {
            var response = await _apiService.PostAsync<PickupPointModel>("PickupPoints", pickupPoint);

            if (response != null)
            {
                MessageBox.Show("ПВЗ успешно добавлен!");
                await LoadData();
            }
            else
            {
                MessageBox.Show("Ошибка при добавлении ПВЗ");
            }
        }

        private async void Update(PickupPointModel pickupPoint)
        {
            var success = await _apiService.PutAsync($"PickupPoints/{pickupPoint.Id}", pickupPoint);

            if (success)
            {
                MessageBox.Show("ПВЗ успешно обновлён!");
                await LoadData();
            }
            else
            {
                MessageBox.Show("Ошибка при обновлении ПВЗ");
            }
        }

        private async Task Delete(object parameter)
        {
            if (parameter is PickupPointModel pp)
            {
                var result = MessageBox.Show(
                    $"Удалить ПВЗ '{pp.Name}'?\n\nВнимание: Это может повлиять на существующие заказы!",
                    "Подтверждение",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _apiService.DeleteAsync($"PickupPoints/{pp.Id}");

                    if (success)
                    {
                        MessageBox.Show("ПВЗ удалён!");
                        await LoadData();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении ПВЗ");
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