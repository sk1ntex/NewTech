using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace NewTech
{
    public partial class MainWindow : Window
    {
        Entities db = new Entities();
        private RequestCardModel selectedCard = null;

        public MainWindow()
        {
            InitializeComponent();
            LoadCards();
        }

        public void LoadCards()
        {
            try
            {
                var requests = db.Requests.OrderByDescending(r => r.CreatedAt).ToList();
                var cardModels = new List<RequestCardModel>();

                foreach (var r in requests)
                {
                    var partner = r.Partners;

                    string partnerType = partner?.PartnerType?.Name ?? "Не указан";

                    decimal total = 0m;

                    foreach (var item in r.RequestItems)
                    {
                        decimal price = (decimal)item.Products.MinPartnerCost;
                        if (price < 0) price = 0;
                        total += item.Quantity * price;
                    }

                    total = Math.Round(total, 2);

                    cardModels.Add(new RequestCardModel
                    {
                        RequestId = r.Id,
                        TitleText = $"{partnerType} | {partner?.Name}",
                        Address = $"Юридический адрес: {partner?.PartnerAddress}",
                        Phone = $"Телефон: {partner?.Telephone}",
                        RatingText = $"Рейтинг: {partner?.Rating}",
                        CostText = $"{total:N2} ₽"
                    });
                }

                CardsList.ItemsSource = cardModels;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки заявок: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Card_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var framework = sender as FrameworkElement;
            if (framework == null) return;

            selectedCard = framework.DataContext as RequestCardModel;

            if (selectedCard != null)
            {
                OpenEditRequestWindow(selectedCard.RequestId);
            }
        }


        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            EditRequestWindow editRequestWindow = new EditRequestWindow();
            editRequestWindow.Show();
            this.Close();
        }

        private void OpenEditRequestWindow(int requestId)
        {
            EditRequestWindow editRequestWindow = new EditRequestWindow(requestId);
            editRequestWindow.Show();
            this.Close();
        }

        private void PartnerProductButton_Click(object sender, RoutedEventArgs e)
        {
            PartnerProductsWindow partnerWindow = new PartnerProductsWindow();
            partnerWindow.Show();
            this.Close();
        }

        private void CalculatorButton_Click(object sender, RoutedEventArgs e)
        {
            MaterialCalculatorWindow materialCalculatorWindow = new MaterialCalculatorWindow();
            materialCalculatorWindow.Show();
            this.Close();
        }
    }

    public class RequestCardModel
    {
        public int RequestId { get; set; }
        public string TitleText { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string RatingText { get; set; }
        public string CostText { get; set; }
    }
}
