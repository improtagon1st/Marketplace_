using MarketplaceWPF.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MarketplaceWPF.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is RegisterViewModel viewModel)
            {
                viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
            }
        }

        private void PersonalDataLink_Click(object sender, RoutedEventArgs e)
        {
            ShowLegalDocument("Согласие на обработку персональных данных");
        }

        private void UserAgreementLink_Click(object sender, RoutedEventArgs e)
        {
            ShowLegalDocument("Пользовательское соглашение");
        }

        private void ShowLegalDocument(string title)
        {
            var window = new ConsentDocumentWindow(title)
            {
                Owner = this
            };

            window.ShowDialog();
        }
    }
}
