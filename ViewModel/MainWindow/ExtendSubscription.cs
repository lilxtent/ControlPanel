using ControlPanel.Model;
using ControlPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControlPanel.ViewModel.MainWindow
{
    public class ExtendSubscription
    {
        public Grid Grid { get; private set; }
        private ControlPanel.MainWindow CurrWindow { get; set; }
        public ComboBox ExtendPeriodBox { get; set; }
        private DatePicker DateStartPeriod { get; set; }
        private DatePicker dateEndPeriod { get; set; }
        private ComboBox comboBox { get; set; }
        private ComboBox comboBoxSubscription { get; set; }
        private TextBox costTextBox { get; set; }
        private TextBox costPerMonthTextBox { get; set; }
        private ClientModel currClient { get; set; }
        private ApplicationContext DB { get; set; }

        private List<int> costPerMonthArray { get; set; }


        private int comboBoxAnotherIndex;
        private bool isDatePicerChangeInsideComboBox = false;

        public ExtendSubscription(ControlPanel.MainWindow CurrWindow)
        {
            this.CurrWindow = CurrWindow;
            DB = new();
            currClient = (ClientModel)((CurrWindow.lbClients.SelectedItem as Grid).DataContext);

            Grid = new Grid();
            // разметили сетку 8 строк на 2 колонки
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
            int NRow = 8;
            for (int i = 0; i < NRow; i++)
                Grid.RowDefinitions.Add(new RowDefinition());

            // инициализация объекты с датой
            DateStartPeriod = new DatePicker()
            {
                SelectedDate = currClient.DateLastPayment == default ? DateTime.Today : currClient.DateLastPayment
            };
            dateEndPeriod = new DatePicker() { IsEnabled = false };
            DateStartPeriod.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);
            //dateEndPeriod.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);

            // инициализировали текстовые поля
            Label startLable = new Label() { Content = "Начало:" };
            Label endLable = new Label() { Content = "Конец:" };
            Label nameLable = new Label() { Content = "Продлить абонемент:" };
            Label chooseLable = new Label() { Content = "Выберите срок:" };
            Label chooseSubscriptionLable = new Label() { Content = "Выберите абонемент:" };
            Label costPerMonthLable = new Label() { Content = "сумма р/мес." };
            Label costLable = new Label() { Content = "сумма оплаты:" };

            // инициализируем combo box для выбора абонемента
            comboBoxSubscription = new ComboBox();
            comboBoxSubscription.SelectionChanged += new SelectionChangedEventHandler(ComboBoxSubscription_SelectionChanged);
            costPerMonthArray = new ();
            foreach (SubscriptionModel subscription in DB.Subscriptions.ToList())
            {
                comboBoxSubscription.Items.Add(new Label() { Content = subscription.Subscription });
                costPerMonthArray.Add(subscription.Cost);
            }
            //comboBoxSubscription.SelectedIndex = 0; // выбираем по умолчанию

            // инициализируем combo box для выбора продолжительности абонемента
            comboBox = new ComboBox();
            //comboBox.SelectedIndex = 0; // изначально выбран "1 месяц"
            comboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            string[] mounths = { "1 месяц", "2 месяца", "3 месяца", "6 месяцев", "1 год"};
            foreach (string content in mounths)
                comboBox.Items.Add(new Label() { Content = content });
            // инициализируем окно ввода суммы оплаты
            costTextBox = new() { Margin = new Thickness(0, 3, 3, 3), IsEnabled = false};
            // инициализируем окно  суммы оплаты
            costPerMonthTextBox = new() { Margin = new Thickness(0, 3, 3, 3), IsEnabled = false };
            // иницализируем кнопку продлить
            Button ButSave = new Button() { Content = "продлить", Margin = new Thickness(3, 3, 3, 3) };
            ButSave.Click += new RoutedEventHandler(butExtend_Click);

            Grid.Children.Add(nameLable);
            Grid.Children.Add(startLable);
            Grid.Children.Add(endLable);
            Grid.Children.Add(DateStartPeriod);
            Grid.Children.Add(dateEndPeriod);
            Grid.Children.Add(comboBox);
            Grid.Children.Add(chooseLable);
            Grid.Children.Add(costLable);
            Grid.Children.Add(costTextBox);
            Grid.Children.Add(ButSave);
            Grid.Children.Add(chooseSubscriptionLable);
            Grid.Children.Add(comboBoxSubscription);
            Grid.Children.Add(costPerMonthLable);
            Grid.Children.Add(costPerMonthTextBox);
            // устанавливаем название
            Grid.SetColumn(nameLable, 0);
            Grid.SetRow(nameLable, 0);

            // устанавливаем надпись старта
            Grid.SetColumn(startLable, 0);
            Grid.SetRow(startLable, 1);
            // устанавливаем дату старта
            Grid.SetColumn(DateStartPeriod, 1);
            Grid.SetRow(DateStartPeriod, 1);

            // устанавливаем надпись конца
            Grid.SetColumn(endLable, 0);
            Grid.SetRow(endLable, 2);
            // устанавливаем дату конца
            Grid.SetColumn(dateEndPeriod, 1);
            Grid.SetRow(dateEndPeriod, 2);

            // устанавливаем комбо бокс название
            Grid.SetColumn(chooseLable, 0);
            Grid.SetRow(chooseLable, 3);
            // устанавливаем комбо бокс
            Grid.SetColumn(comboBox, 1);
            Grid.SetRow(comboBox, 3);
            // устанавливаем комбо бокс абонемента название
            Grid.SetColumn(chooseSubscriptionLable, 0);
            Grid.SetRow(chooseSubscriptionLable, 4);
            // устанавливаем комбо бокс абонемента
            Grid.SetColumn(comboBoxSubscription, 1);
            Grid.SetRow(comboBoxSubscription, 4);
            // устанавливаем название для указания цены за месяц
            Grid.SetColumn(costPerMonthLable, 0);
            Grid.SetRow(costPerMonthLable, 5);
            // устанавливаем текст бокс для указания цены за месяц
            Grid.SetColumn(costPerMonthTextBox, 1);
            Grid.SetRow(costPerMonthTextBox, 5);
            // устанавливаем название для указания цены
            Grid.SetColumn(costLable, 0);
            Grid.SetRow(costLable, 6);
            // устанавливаем текст бокс для указания цены
            Grid.SetColumn(costTextBox, 1);
            Grid.SetRow(costTextBox, 6);
            // устанавливаем кнопку продлить
            Grid.SetColumn(ButSave, 0);
            Grid.SetRow(ButSave, 7);

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] multMounth = { 1, 2, 3, 6, 12 };
            int index = (sender as ComboBox).SelectedIndex;
            if (index != -1 && DateStartPeriod.SelectedDate is not null)
            {
                DateTime start = (DateTime)DateStartPeriod.SelectedDate;
                dateEndPeriod.SelectedDate = start.AddMonths(multMounth[index]); 
                // если у нас уже указан абонемент меняем стоимость
                if (comboBoxSubscription.SelectedIndex != -1)
                    costTextBox.Text = (costPerMonthArray[comboBoxSubscription.SelectedIndex] * multMounth[index]).ToString();
            }
            // указываем что дата поменялась из-за изменений в combo box
            isDatePicerChangeInsideComboBox = true; 
            
        }
        private void ComboBoxSubscription_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] multMounth = { 1, 2, 3, 6, 12 };
            
            int index = (sender as ComboBox).SelectedIndex;
            if (index != -1 && costPerMonthArray.Count != 0 && comboBox.SelectedIndex != -1)
            {
                costPerMonthTextBox.Text = costPerMonthArray[index].ToString();
                if (comboBox is not null)
                    costTextBox.Text = (costPerMonthArray[index] * multMounth[comboBox.SelectedIndex]).ToString();
            }
            //// указываем что дата поменялась из-за изменений в combo box
            //isDatePicerChangeInsideComboBox = true;

        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBox is not null && !isDatePicerChangeInsideComboBox)
                comboBox.SelectedIndex = -1;
            isDatePicerChangeInsideComboBox = false;
        }
        private bool CheckAllError()
        {
            void ThisFieldCantBeEmpty(string fieldName) =>
                MessageBox.Show($"Это поле не может быть пустым: {fieldName}", "Ошибка");

            // обработка пустых полей
            if (DateStartPeriod.SelectedDate == default)
            {
                ThisFieldCantBeEmpty("Старт");
                return false;
            }
            if (dateEndPeriod.SelectedDate == default)
            {
                ThisFieldCantBeEmpty("Конец");
                return false;
            }
            if (costTextBox.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Сумма оплаты");
                return false;
            }
            // обработка суммы на float
            int temp;
            if (!(int.TryParse(costTextBox.Text.Trim(' '), out temp)))
            {
                MessageBox.Show($"Некорректная сумма оплаты!", "Ошибка");
                return false;
            }
            // обработка некорректного периода оплаты
            if (((DateTime)dateEndPeriod.SelectedDate - (DateTime)DateStartPeriod.SelectedDate).Days < 0)
            {
                MessageBox.Show($"Некорректный период оплаты!", "Ошибка");
                return false;
            }
            // смотрим пытаемся ли мы оплатить уже оплаченный период дей

            if (currClient.DateLastPayment != default && currClient.DateLastPayment > DateStartPeriod.SelectedDate)
            {
                MessageBox.Show($"В период который вы оплачиваете входят оплаченные дни!", "Ошибка");
                return false;
            }

            return true;
        }

        private void butExtend_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAllError()) return;

            int id = currClient.ID;

            DateTime timePayment = DateTime.Today;
            timePayment.AddHours(DateTime.Now.Date.Hour);
            // запись в таблицу платежей
            DB.Payments.Add(new PaymentModel(id, DateTime.Now,
                DateStartPeriod.SelectedDate.Value, dateEndPeriod.SelectedDate.Value, int.Parse(costTextBox.Text),
                comboBoxSubscription.Text, int.Parse(costPerMonthTextBox.Text)));
            // обновляем информацию о последнем платеже клиента
            ClientModel NewClient = currClient;
            NewClient.SetDateLastPayment(dateEndPeriod.SelectedDate);
            DB.ClientsModels.Update(NewClient);
            // сохраняем бд
            DB.SaveChanges();
            // после продления очищаем и выводим сообщение
            CurrWindow.spPayment.Children.Clear();
            MessageBox.Show("Абонемент успешно продлен");
            CurrWindow.UpdateClientsList(CurrWindow.lbClients); // обновляем листбокс
            CurrWindow.ShowAllClientsShortData(CurrWindow.lbClients);

        }
        public void Show() => CurrWindow.spPayment.Children.Add(Grid);

    }

}
