using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.ComponentModel;
using System.Net.Mail;
using System.Windows;
using System.Windows.Input;

namespace MarketplaceWPF.ViewModels
{
    public class RegisterViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService;

        private string _fullName = string.Empty;
        private string _email = string.Empty;
        private string _phone = string.Empty;
        private string _password = string.Empty;
        private string _confirmPassword = string.Empty;
        private bool _isPersonalDataConsentAccepted;
        private bool _isBusy;

        public string FullName
        {
            get => _fullName;
            set
            {
                _fullName = value;
                OnPropertyChanged(nameof(FullName));
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public string Phone
        {
            get => _phone;
            set
            {
                _phone = value;
                OnPropertyChanged(nameof(Phone));
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

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged(nameof(IsBusy));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsPersonalDataConsentAccepted
        {
            get => _isPersonalDataConsentAccepted;
            set
            {
                _isPersonalDataConsentAccepted = value;
                OnPropertyChanged(nameof(IsPersonalDataConsentAccepted));
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public ICommand RegisterCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public RegisterViewModel()
        {
            _apiService = new ApiService();

            RegisterCommand = new RelayCommand(async param => await Register(param as Window), _ => !IsBusy);
            BackToLoginCommand = new RelayCommand(param => CloseWindow(param as Window), _ => !IsBusy);
        }

        private async Task Register(Window? window)
        {
            if (IsBusy)
            {
                return;
            }

            var fullName = FullName?.Trim();
            var email = Email?.Trim().ToLowerInvariant();
            var phone = Phone?.Trim();

            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            if (!IsEmailValid(email))
            {
                MessageBox.Show("Введите корректный email");
                return;
            }

            if (!IsPhoneValid(phone))
            {
                MessageBox.Show("Введите корректный номер телефона");
                return;
            }

            if (Password.Length < 6)
            {
                MessageBox.Show("Пароль должен содержать минимум 6 символов");
                return;
            }

            if (Password != ConfirmPassword)
            {
                MessageBox.Show("Пароли не совпадают");
                return;
            }

            if (!IsPersonalDataConsentAccepted)
            {
                MessageBox.Show("Необходимо согласие на обработку персональных данных");
                return;
            }

            var request = new RegisterRequest
            {
                FullName = fullName,
                Email = email,
                Phone = phone,
                Password = Password
            };

            try
            {
                IsBusy = true;
                var response = await _apiService.PostAsync<string>("Auth/register", request);

                if (!string.IsNullOrWhiteSpace(response))
                {
                    MessageBox.Show("Регистрация успешна. Теперь войдите в систему.");
                    CloseWindow(window);
                }
                else
                {
                    MessageBox.Show("Не удалось зарегистрироваться");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private static bool IsEmailValid(string email)
        {
            try
            {
                var address = new MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsPhoneValid(string phone)
        {
            var digits = new string(phone.Where(char.IsDigit).ToArray());
            if (digits.Length < 10 || digits.Length > 15)
            {
                return false;
            }

            return phone.All(ch => char.IsDigit(ch) || ch == '+' || ch == ' ' || ch == '-' || ch == '(' || ch == ')');
        }

        private static void CloseWindow(Window? window)
        {
            window?.Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
