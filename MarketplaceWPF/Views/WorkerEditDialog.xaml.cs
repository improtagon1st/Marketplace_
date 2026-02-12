using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class WorkerEditDialog : Window
    {
        private readonly ApiService _apiService;
        private readonly WorkerModel _existingWorker;
        private readonly bool _isEditMode;

        // Конструктор для добавления нового работника
        public WorkerEditDialog()
        {
            InitializeComponent();
            _apiService = new ApiService { Token = UserSession.Token };
            _isEditMode = false;

            TitleText.Text = "Добавить работника";
            PasswordLabel.Text = "Пароль *";
            PasswordHint.Visibility = Visibility.Collapsed;

            LoadPickupPoints();
        }

        // Конструктор для редактирования существующего работника
        public WorkerEditDialog(WorkerModel worker)
        {
            InitializeComponent();
            _apiService = new ApiService { Token = UserSession.Token };
            _existingWorker = worker;
            _isEditMode = true;

            TitleText.Text = "Редактировать работника";
            PasswordLabel.Text = "Новый пароль (опционально)";
            PasswordHint.Visibility = Visibility.Visible;

            // Заполняем поля данными работника
            EmailInput.Text = worker.Email;
            FullNameInput.Text = worker.FullName;
            PhoneInput.Text = worker.Phone;

            LoadPickupPoints(worker.PickupPointId);
        }

        private async void LoadPickupPoints(int? selectedPickupPointId = null)
        {
            var pickupPoints = await _apiService.GetAsync<List<PickupPointModel>>("PickupPoints");

            if (pickupPoints != null && pickupPoints.Any())
            {
                // Добавляем опцию "Без привязки"
                var allOptions = new List<PickupPointModel>
                {
                    new PickupPointModel { Id = 0, Name = "-- Без привязки к ПВЗ --" }
                };
                allOptions.AddRange(pickupPoints);

                PickupPointCombo.ItemsSource = allOptions;

                // Устанавливаем выбранный ПВЗ
                if (selectedPickupPointId.HasValue)
                {
                    PickupPointCombo.SelectedItem = allOptions.FirstOrDefault(p => p.Id == selectedPickupPointId.Value);
                }
                else
                {
                    PickupPointCombo.SelectedIndex = 0; // "Без привязки"
                }
            }
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(EmailInput.Text))
            {
                MessageBox.Show("Введите email");
                return;
            }

            if (!_isEditMode && string.IsNullOrWhiteSpace(PasswordInput.Password))
            {
                MessageBox.Show("Введите пароль");
                return;
            }

            if (string.IsNullOrWhiteSpace(FullNameInput.Text))
            {
                MessageBox.Show("Введите ФИО");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneInput.Text))
            {
                MessageBox.Show("Введите телефон");
                return;
            }

            // Определяем выбранный ПВЗ
            int? pickupPointId = null;
            if (PickupPointCombo.SelectedItem is PickupPointModel selectedPP && selectedPP.Id > 0)
            {
                pickupPointId = selectedPP.Id;
            }

            if (_isEditMode)
            {
                // Редактирование существующего работника
                var request = new UpdateWorkerRequest
                {
                    Email = EmailInput.Text.Trim(),
                    FullName = FullNameInput.Text.Trim(),
                    Phone = PhoneInput.Text.Trim(),
                    PickupPointId = pickupPointId,
                    NewPassword = string.IsNullOrWhiteSpace(PasswordInput.Password) ? null : PasswordInput.Password
                };

                var success = await _apiService.PutAsync($"Workers/{_existingWorker.Id}", request);

                if (success)
                {
                    MessageBox.Show("Работник успешно обновлён");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при обновлении работника");
                }
            }
            else
            {
                // Создание нового работника
                var request = new CreateWorkerRequest
                {
                    Email = EmailInput.Text.Trim(),
                    Password = PasswordInput.Password,
                    FullName = FullNameInput.Text.Trim(),
                    Phone = PhoneInput.Text.Trim(),
                    PickupPointId = pickupPointId
                };

                var response = await _apiService.PostAsync<object>("Workers", request);

                if (response != null)
                {
                    MessageBox.Show("Работник успешно создан");
                    DialogResult = true;
                    Close();
                }
                else
                {
                    MessageBox.Show("Ошибка при создании работника");
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}