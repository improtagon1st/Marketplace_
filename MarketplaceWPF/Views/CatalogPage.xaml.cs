using MarketplaceWPF.Helpers;
using MarketplaceWPF.Models;
using MarketplaceWPF.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Net.Http;

namespace MarketplaceWPF.Views
{
    public partial class CatalogPage : Page
    {
        public CatalogPage()
        {
            InitializeComponent();
        }

        private void ProductCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is System.Windows.FrameworkElement element &&
                element.Tag is ProductModel product)
            {
                NavigationService?.Navigate(new ProductDetailsPage(product));
            }
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ProductModel product)
            {
                NavigationService?.Navigate(new ProductDetailsPage(product));
            }
        }

        private async void AddToCart_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is ProductModel product)
            {
                var request = new AddToCartRequest
                {
                    ProductId = product.Id,
                    Quantity = 1
                };

                try
                {
                    var httpClient = new System.Net.Http.HttpClient();
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", UserSession.Token);

                    var json = System.Text.Json.JsonSerializer.Serialize(request);
                    var content = new System.Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json");

                    var response = await httpClient.PostAsync("https://localhost:7093/api/Cart", content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"✅ '{product.Name}' добавлен в корзину!");
                    }
                    else
                    {
                        MessageBox.Show("Ошибка при добавлении в корзину");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка: {ex.Message}");
                }
            }
        }
    }
}