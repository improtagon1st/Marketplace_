using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using MarketplaceWPF.Views;
using System.Linq;


namespace MarketplaceWPF.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        private string _email;
        private string _password;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }

        public LoginViewModel()
        {
            _apiService = new ApiService();

            LoginCommand = new RelayCommand(async (param) => await Login());
            RegisterCommand = new RelayCommand((param) => OpenRegister());
        }

        private async Task Login()
        {
            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            var request = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            var response = await _apiService.PostAsync<LoginResponse>("Auth/login", request);

            if (response != null)
            {
                // Сохраняем данные пользователя
                UserSession.Token = response.Token;
                UserSession.UserId = response.UserId;
                UserSession.Email = response.Email;
                UserSession.FullName = response.FullName;
                UserSession.Role = response.Role;
                UserSession.PickupPointId = response.PickupPointId; // <- Добавь это

                // Открываем главное окно
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // Закрываем окно входа
                Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w is Views.LoginWindow)?.Close();
            }
            else
            {
                MessageBox.Show("Неверный email или пароль");
            }
        }

        private void OpenRegister()
        {
            var owner = Application.Current.Windows
                .OfType<Window>()
                .FirstOrDefault(w => w is Views.LoginWindow);

            var registerWindow = new RegisterWindow();

            if (owner != null)
            {
                registerWindow.Owner = owner;
            }

            registerWindow.ShowDialog();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
