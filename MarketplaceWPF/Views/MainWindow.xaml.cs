using MarketplaceWPF.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarketplaceWPF.Views
{
    public partial class MainWindow : Window
    {
        private List<Button> _menuButtons;

        public MainWindow()
        {
            InitializeComponent();
            SetupMenu();
        }

        private void SetupMenu()
        {
            // Показываем имя пользователя
            UserNameText.Text = $"{UserSession.FullName} ({UserSession.Role})";

            // Собираем все кнопки меню в список
            _menuButtons = new List<Button>
                {
                    BtnCatalog, BtnCart, BtnOrders, BtnProfile, // Customer
                    BtnPickupOrders, BtnScanQR, // Worker
                    BtnProducts, BtnCategories, BtnPickupPoints, BtnWorkers, BtnAllOrders // Admin
                };


            // Показываем меню в зависимости от роли
            if (UserSession.IsCustomer)
            {
                CustomerMenu.Visibility = Visibility.Visible;
                _menuButtons.AddRange(new[] { BtnCatalog, BtnOrders, BtnProfile });

                // По умолчанию открываем каталог
                ShowCatalog_Click(null, null);
            }
            else if (UserSession.IsWorker)
            {
                WorkerMenu.Visibility = Visibility.Visible;
                _menuButtons.AddRange(new[] { BtnPickupOrders, BtnScanQR });

                // По умолчанию открываем заказы на ПВЗ
                ShowPickupOrders_Click(null, null);
            }
            else if (UserSession.IsAdmin)
            {
                AdminMenu.Visibility = Visibility.Visible;
                _menuButtons.AddRange(new[] { BtnProducts, BtnCategories, BtnPickupPoints, BtnAllOrders });

                // По умолчанию открываем управление товарами
                ShowProducts_Click(null, null);
            }
        }

        private void SetActiveButton(Button activeButton)
        {
            // Сбрасываем фон у всех кнопок
            foreach (var button in _menuButtons)
            {
                button.Background = new SolidColorBrush(Colors.Transparent);
            }

            // Подсвечиваем активную кнопку
            if (activeButton != null)
            {
                activeButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1565C0"));
            }
        }

        // Customer меню
        private void ShowCatalog_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CatalogPage());
            SetActiveButton(BtnCatalog);
        }

        private void ShowOrders_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new OrdersPage());
            SetActiveButton(BtnOrders);
        }
        private void ShowWorkers_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new WorkerManagementPage());
            SetActiveButton(BtnWorkers);
        }
        private void ShowCart_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CartPage());
            SetActiveButton(BtnCart);
        }

        private void ShowProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Профиль - создадим дальше");
            SetActiveButton(BtnProfile);
        }

        // Worker меню
        private void ShowPickupOrders_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new PickupPointOrdersPage());
            SetActiveButton(BtnPickupOrders);
        }

        private void ShowScanQR_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new PickupWorkerPage());
            SetActiveButton(BtnScanQR);
        }

        // Admin меню
        private void ShowProducts_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new ProductManagementPage());
            SetActiveButton(BtnProducts);
        }

        private void ShowCategories_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CategoriesManagementPage());
            SetActiveButton(BtnCategories);
        }

        private void ShowPickupPoints_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new PickupPointManagementPage());
            SetActiveButton(BtnPickupPoints);
        }

        private void ShowAllOrders_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new AllOrdersPage());
            SetActiveButton(BtnAllOrders);
        }

        // Выход
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}