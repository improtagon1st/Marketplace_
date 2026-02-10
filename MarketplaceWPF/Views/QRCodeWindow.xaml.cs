using MarketplaceWPF.Models;
using QRCoder;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace MarketplaceWPF.Views
{
    public partial class QRCodeWindow : Window
    {
        public int OrderId { get; set; }
        public decimal TotalPrice { get; set; }
        public string StatusText { get; set; }
        public string PickupPointName { get; set; }
        public string PickupPointAddress { get; set; }
        public string QRCodeText { get; set; }

        public QRCodeWindow(OrderModel order)
        {
            InitializeComponent();

            OrderId = order.Id;
            TotalPrice = order.TotalPrice;
            StatusText = GetStatusText(order.Status);
            PickupPointName = order.PickupPointName;
            PickupPointAddress = order.PickupPointAddress;
            QRCodeText = order.QRCode;

            DataContext = this;

            GenerateQRCode(order.QRCode);
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "Created" => "Создан (ожидает доставки)",
                "Delivered" => "Доставлен в ПВЗ",
                "PickedUp" => "Получен",
                "Cancelled" => "Отменён",
                _ => status
            };
        }

        private void GenerateQRCode(string text)
        {
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            {
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
                using (QRCode qrCode = new QRCode(qrCodeData))
                {
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);

                    // Конвертируем Bitmap в BitmapImage для WPF
                    using (MemoryStream memory = new MemoryStream())
                    {
                        qrCodeImage.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                        memory.Position = 0;

                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = memory;
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();

                        QRCodeImage.Source = bitmapImage;
                    }
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}