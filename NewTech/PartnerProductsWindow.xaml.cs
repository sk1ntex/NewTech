using System.Linq;
using System.Windows;

namespace NewTech
{
    public partial class PartnerProductsWindow : Window
    {
        private Entities db = new Entities();

        public PartnerProductsWindow()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            try
            {
                var products = db.Products
                                 .Select(p => new
                                 {
                                     p.Name,
                                     StockQuantity = p.StockQuantity,
                                     p.MinPartnerCost
                                 })
                                 .ToList();

                ProductsGrid.ItemsSource = products;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки продукции: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
