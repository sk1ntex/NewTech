using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace NewTech
{
    public partial class EditRequestWindow : Window
    {
        private Entities db = new Entities();
        private int? editingRequestId = null;
        private Partners currentPartner = null;
        private Requests currentRequest = null;

        // Буфер для редактирования элементов заявки
        public ObservableCollection<RequestItems> EditingItems { get; set; } = new ObservableCollection<RequestItems>();

        // список продуктов для ComboBox в DataGrid
        public List<Products> ProductsForCombo { get; set; } = new List<Products>();

        public EditRequestWindow()
        {
            InitializeComponent();
            LoadLookups();
            ItemsGrid.ItemsSource = EditingItems;
            DataContext = this;
        }

        public EditRequestWindow(int requestId) : this()
        {
            LoadForEdit(requestId);
        }

        private void LoadLookups()
        {
            try
            {
                ProductsForCombo = db.Products.OrderBy(p => p.Name).ToList();
                PartnerTypeCombo.ItemsSource = db.PartnerType.OrderBy(pt => pt.Name).ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке справочников: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadForEdit(int requestId)
        {
            try
            {
                currentRequest = db.Requests.FirstOrDefault(r => r.Id == requestId);
                if (currentRequest == null)
                {
                    MessageBox.Show("Заявка не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.DialogResult = false;
                    this.Close();
                    return;
                }

                // партнер
                currentPartner = currentRequest.Partners;

                // Заполняем поля партнёра
                if (currentPartner != null)
                {
                    // Выбираем тип партнёра в ComboBox
                    if (currentPartner.PartnerType != null)
                        PartnerTypeCombo.SelectedValue = currentPartner.PartnerType.Id;

                    NameBox.Text = currentPartner.Name;
                    DirectorBox.Text = currentPartner.Director;
                    AddressBox.Text = currentPartner.PartnerAddress;
                    RatingBox.Text = currentPartner.Rating.ToString();
                    PhoneBox.Text = currentPartner.Telephone;
                    EmailBox.Text = currentPartner.Email;
                }

                // Загружаем элементы заявки
                EditingItems.Clear();
                foreach (var it in currentRequest.RequestItems.ToList())
                {
                    EditingItems.Add(new RequestItems
                    {
                        Id = it.Id,
                        RequestId = it.RequestId,
                        ProductId = it.ProductId,
                        Quantity = it.Quantity,
                        Products = it.Products
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке заявки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddItemBtn_Click(object sender, RoutedEventArgs e)
        {
            // добавляем новую строку с первым продуктом
            var first = ProductsForCombo.FirstOrDefault();
            if (first == null)
            {
                MessageBox.Show("Нет доступных продуктов для добавления.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var newItem = new RequestItems
            {
                ProductId = first.Id,
                Products = first,
                Quantity = 1
            };

            EditingItems.Add(newItem);
        }

        private void RemoveItemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (ItemsGrid.SelectedItem is RequestItems sel)
            {
                EditingItems.Remove(sel);
            }
            else
            {
                MessageBox.Show("Выберите строку для удаления.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private bool ValidatePartnerFields(out string error)
        {
            error = null;

            if (PartnerTypeCombo.SelectedItem == null)
            {
                error = "Выберите тип партнёра.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(NameBox.Text))
            {
                error = "Введите наименование партнёра.";
                return false;
            }

            if (!int.TryParse(RatingBox.Text, out int rating) || rating < 0)
            {
                error = "Рейтинг должен быть целым неотрицательным числом.";
                return false;
            }

            if (rating > 10)
            {
                error = "Рейтинг должен быть от 0 до 10.";
                return false;
            }

            // проверка email
            if (!string.IsNullOrWhiteSpace(EmailBox.Text) && (!EmailBox.Text.Contains("@") || !EmailBox.Text.Contains(".")))
            {
                error = "Введите корректный email.";
                return false;
            }

            return true;
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidatePartnerFields(out string err))
                {
                    MessageBox.Show(err, "Ошибка ввода", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Собираем данные партнёра
                int partnerTypeId = (int)PartnerTypeCombo.SelectedValue;
                string name = NameBox.Text.Trim();
                string director = DirectorBox.Text.Trim();
                string address = AddressBox.Text.Trim();
                int rating = int.Parse(RatingBox.Text.Trim());
                string phone = PhoneBox.Text.Trim();
                string email = EmailBox.Text.Trim();

                if (currentRequest == null)
                {
                    var newPartner = new Partners
                    {
                        PartnerTypeId = partnerTypeId,
                        Name = name,
                        Director = director,
                        PartnerAddress = address,
                        Rating = rating,
                        Telephone = phone,
                        Email = email
                    };
                    db.Partners.Add(newPartner);
                    db.SaveChanges();

                    var newRequest = new Requests
                    {
                        PartnerId = newPartner.Id,
                        CreatedAt = DateTime.Now,
                        TotalCost = 0
                    };
                    db.Requests.Add(newRequest);
                    db.SaveChanges();

                    foreach (var it in EditingItems)
                    {
                        var ri = new RequestItems
                        {
                            RequestId = newRequest.Id,
                            ProductId = it.ProductId,
                            Quantity = it.Quantity
                        };
                        db.RequestItems.Add(ri);
                    }

                    newRequest.TotalCost = RecalculateTotalForRequest(newRequest.Id);
                    db.SaveChanges();
                }
                else
                {
                    currentPartner.PartnerTypeId = partnerTypeId;
                    currentPartner.Name = name;
                    currentPartner.Director = director;
                    currentPartner.PartnerAddress = address;
                    currentPartner.Rating = rating;
                    currentPartner.Telephone = phone;
                    currentPartner.Email = email;

                    var existingItems = db.RequestItems.Where(ri => ri.RequestId == currentRequest.Id).ToList();
                    db.RequestItems.RemoveRange(existingItems);
                    db.SaveChanges();

                    foreach (var it in EditingItems)
                    {
                        var newRi = new RequestItems
                        {
                            RequestId = currentRequest.Id,
                            ProductId = it.ProductId,
                            Quantity = it.Quantity
                        };
                        db.RequestItems.Add(newRi);
                    }
                    db.SaveChanges();

                    currentRequest.TotalCost = RecalculateTotalForRequest(currentRequest.Id);
                    db.SaveChanges();
                }

                MessageBox.Show("Сохранение успешно.", "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                // Создаем новое главное окно с актуальными данными
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close(); // закрываем окно редактирования
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private double RecalculateTotalForRequest(int requestId)
        {
            double total = 0.0;

            var items = db.RequestItems.Where(ri => ri.RequestId == requestId).ToList();
            foreach (var it in items)
            {
                var prod = db.Products.FirstOrDefault(p => p.Id == it.ProductId);
                if (prod != null)
                {
                    double price = prod.MinPartnerCost;
                    if (price < 0) price = 0;
                    total += it.Quantity * price;
                }
            }

            // округляем до сотых
            total = Math.Round(total, 2);
            return total;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close(); // закрываем окно редактирования
        }
    }
}
