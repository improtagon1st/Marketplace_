using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MarketplaceWPF.Helpers;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>


namespace MarketplaceWPF.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SetupMenu();
        }

        private void SetupMenu()
        {
            // Показываем имя пользователя
            UserNameText.Text = $"{UserSession.FullName} ({UserSession.Role})";

            // Показываем меню в зависимости от роли
            if (UserSession.IsCustomer)
            {
                CustomerMenu.Visibility = Visibility.Visible;
            }
            else if (UserSession.IsWorker)
            {
                WorkerMenu.Visibility = Visibility.Visible;
            }
            else if (UserSession.IsAdmin)
            {
                AdminMenu.Visibility = Visibility.Visible;
            }
        }

        // Customer меню
        private void ShowCatalog_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new CatalogPage());
        }

        private void ShowOrders_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(new OrdersPage());
        }

        private void ShowProfile_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Профиль - создадим дальше");
        }

        // Worker меню
        private void ShowPickupOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Заказы на ПВЗ - создадим дальше");
        }

        private void ShowScanQR_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Сканировать QR - создадим дальше");
        }

        // Admin меню
        private void ShowProducts_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Управление товарами - создадим дальше");
        }

        private void ShowCategories_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Категории - создадим дальше");
        }

        private void ShowPickupPoints_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пункты выдачи - создадим дальше");
        }

        private void ShowAllOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Все заказы - создадим дальше");
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