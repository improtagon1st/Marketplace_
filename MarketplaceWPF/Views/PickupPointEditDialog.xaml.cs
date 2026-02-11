using MarketplaceWPF.Models;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class PickupPointEditDialog : Window
    {
        public PickupPointModel PickupPoint { get; private set; }
        private bool _isEditMode;

        // Конструктор для добавления нового ПВЗ
        public PickupPointEditDialog()
        {
            InitializeComponent();

            _isEditMode = false;
            TitleText.Text = "Добавить ПВЗ";
        }

        // Конструктор для редактирования существующего ПВЗ
        public PickupPointEditDialog(PickupPointModel pickupPoint)
        {
            InitializeComponent();

            _isEditMode = true;
            TitleText.Text = "Редактировать ПВЗ";

            // Заполняем поля данными ПВЗ
            NameBox.Text = pickupPoint.Name;
            AddressBox.Text = pickupPoint.Address;
            PhoneBox.Text = pickupPoint.Phone;
            WorkingHoursBox.Text = pickupPoint.WorkingHours;

            PickupPoint = pickupPoint;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // Валидация
            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                MessageBox.Show("Введите название ПВЗ");
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Введите адрес");
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text))
            {
                MessageBox.Show("Введите телефон");
                return;
            }

            if (string.IsNullOrWhiteSpace(WorkingHoursBox.Text))
            {
                MessageBox.Show("Введите часы работы");
                return;
            }

            // Создаём или обновляем ПВЗ
            if (_isEditMode)
            {
                // Обновляем существующий ПВЗ
                PickupPoint.Name = NameBox.Text;
                PickupPoint.Address = AddressBox.Text;
                PickupPoint.Phone = PhoneBox.Text;
                PickupPoint.WorkingHours = WorkingHoursBox.Text;
            }
            else
            {
                // Создаём новый ПВЗ
                PickupPoint = new PickupPointModel
                {
                    Name = NameBox.Text,
                    Address = AddressBox.Text,
                    Phone = PhoneBox.Text,
                    WorkingHours = WorkingHoursBox.Text
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