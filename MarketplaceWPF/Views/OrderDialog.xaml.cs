using MarketplaceWPF.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class OrderDialog : Window
    {
        public PickupPointModel SelectedPickupPoint { get; private set; }
        public int Quantity { get; private set; }

        private ProductModel _product;

        public OrderDialog(List<PickupPointModel> pickupPoints, ProductModel product)
        {
            InitializeComponent();

            _product = product;

            // Заполняем информацию о товаре
            ProductNameText.Text = product.Name;
            ProductPriceText.Text = $"Цена: {product.Price} руб.";

            // Заполняем список ПВЗ
            PickupPointCombo.ItemsSource = pickupPoints;
            if (pickupPoints.Any())
            {
                PickupPointCombo.SelectedIndex = 0;
                UpdatePickupInfo();
            }

            PickupPointCombo.SelectionChanged += (s, e) => UpdatePickupInfo();
        }

        private void UpdatePickupInfo()
        {
            if (PickupPointCombo.SelectedItem is PickupPointModel pp)
            {
                PickupAddressText.Text = $"📍 {pp.Address}";
                PickupPhoneText.Text = $"📞 {pp.Phone}";
                PickupHoursText.Text = $"🕐 {pp.WorkingHours}";
            }
        }

        private void Order_Click(object sender, RoutedEventArgs e)
        {
            if (PickupPointCombo.SelectedItem == null)
            {
                MessageBox.Show("Выберите пункт выдачи");
                return;
            }

            if (!int.TryParse(QuantityBox.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество");
                return;
            }

            if (quantity > _product.Stock)
            {
                MessageBox.Show($"На складе только {_product.Stock} шт.");
                return;
            }

            SelectedPickupPoint = (PickupPointModel)PickupPointCombo.SelectedItem;
            Quantity = quantity;
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