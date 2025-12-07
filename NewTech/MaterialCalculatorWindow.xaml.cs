using System;
using System.Linq;
using System.Windows;

namespace NewTech
{
    public partial class MaterialCalculatorWindow : Window
    {
        private Entities db = new Entities();

        public MaterialCalculatorWindow()
        {
            InitializeComponent();
            LoadLookups();
        }

        private void LoadLookups()
        {
            try
            {
                ProductTypeCombo.ItemsSource = db.ProductType.OrderBy(p => p.Name).ToList();
                MaterialTypeCombo.ItemsSource = db.MaterialType.OrderBy(m => m.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки типов: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ProductTypeCombo.SelectedItem == null || MaterialTypeCombo.SelectedItem == null)
                {
                    MessageBox.Show("Выберите тип продукции и тип материала.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int productTypeId = (int)ProductTypeCombo.SelectedValue;
                int materialTypeId = (int)MaterialTypeCombo.SelectedValue;

                if (!int.TryParse(RequiredQuantityBox.Text, out int requiredQuantity) ||
                    !int.TryParse(StockQuantityBox.Text, out int stockQuantity) ||
                    !double.TryParse(Param1Box.Text, out double param1) ||
                    !double.TryParse(Param2Box.Text, out double param2))
                {
                    MessageBox.Show("Введите корректные числовые значения.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int result = MaterialCalculator.CalculateRequiredMaterial(productTypeId, materialTypeId, requiredQuantity, stockQuantity, param1, param2);

                if (result == -1)
                    ResultText.Text = "Ошибка расчёта";
                else
                    ResultText.Text = $"Необходимо материала: {result}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
