using System.Windows;

namespace MarketplaceWPF.Views
{
    public partial class ConsentDocumentWindow : Window
    {
        public ConsentDocumentWindow(string title)
        {
            InitializeComponent();
            Title = title;
            TitleTextBlock.Text = title;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
