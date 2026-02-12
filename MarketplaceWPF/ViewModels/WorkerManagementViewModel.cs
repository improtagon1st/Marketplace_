using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class WorkerManagementViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;
        private ObservableCollection<WorkerModel> _workers;

        public ObservableCollection<WorkerModel> Workers
        {
            get => _workers;
            set
            {
                _workers = value;
                OnPropertyChanged(nameof(Workers));
            }
        }

        public ICommand AddWorkerCommand { get; }
        public ICommand EditWorkerCommand { get; }
        public ICommand DeleteWorkerCommand { get; }
        public ICommand RefreshCommand { get; }

        public WorkerManagementViewModel()
        {
            _apiService = new ApiService { Token = UserSession.Token };
            Workers = new ObservableCollection<WorkerModel>();

            AddWorkerCommand = new RelayCommand((param) => AddWorker());
            EditWorkerCommand = new RelayCommand((param) => EditWorker(param));
            DeleteWorkerCommand = new RelayCommand((param) => DeleteWorker(param));
            RefreshCommand = new RelayCommand(async (param) => await LoadWorkers());

            LoadWorkers();
        }

        private async System.Threading.Tasks.Task LoadWorkers()
        {
            var workers = await _apiService.GetAsync<System.Collections.Generic.List<WorkerModel>>("Workers");

            if (workers != null)
            {
                Workers.Clear();
                foreach (var worker in workers)
                {
                    Workers.Add(worker);
                }
            }
        }

        private void AddWorker()
        {
            var dialog = new Views.WorkerEditDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadWorkers();
            }
        }

        private void EditWorker(object parameter)
        {
            if (parameter is WorkerModel worker)
            {
                var dialog = new Views.WorkerEditDialog(worker);
                if (dialog.ShowDialog() == true)
                {
                    LoadWorkers();
                }
            }
        }

        private async void DeleteWorker(object parameter)
        {
            if (parameter is WorkerModel worker)
            {
                var result = MessageBox.Show(
                    $"Удалить работника '{worker.FullName}'?\n\nЭто действие нельзя отменить!",
                    "Подтверждение удаления",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    var success = await _apiService.DeleteAsync($"Workers/{worker.Id}");

                    if (success)
                    {
                        MessageBox.Show("Работник удалён");
                        await LoadWorkers();
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при удалении работника");
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