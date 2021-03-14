using ControlPanel.Model;
using ControlPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPanel.ViewModel.MainWindow
{
    class ButtonExtendSubscription
    {
        private Button LocalButton { get; set; }
        private ControlPanel.MainWindow CurrWindow { get; set; }

        public ButtonExtendSubscription(ControlPanel.MainWindow CurrWindow)
        {
            this.CurrWindow = CurrWindow;

            LocalButton = new Button()
            {
                Content = $"продлить абонемент",
                Margin = new Thickness(0, 0, 5, 0),
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 244, 137)),
            };
            LocalButton.Click += new RoutedEventHandler(butExtendSubscription_Click);
        }
        private void butExtendSubscription_Click(object sender, RoutedEventArgs e)
        {
        }
    }
    public class ExtendSubscription
    {
        public Grid Grid { get; private set; }
        private ControlPanel.MainWindow CurrWindow { get; set; }
        public ComboBox ExtendPeriodBox { get; set; }
        private DatePicker DateStartPeriod { get; set; }
        private DatePicker dateEndPeriod { get; set; }
        private ComboBox comboBox { get; set; }
        private TextBox costTextBox { get; set; }
        private ClientModel currClient { get; set; }
        private ApplicationContext DB { get; set; }



        private int comboBoxAnotherIndex;

        public ExtendSubscription(ControlPanel.MainWindow CurrWindow)
        {
            this.CurrWindow = CurrWindow;
            DB = new();
            ListBoxItem lbi = ((CurrWindow.lbClients as ListBox).SelectedItem as ListBoxItem);
            currClient = (lbi.Content as ClientModelInfo).clientModel;

            Grid = new Grid();
            // разметили сетку 6 строк на 2 колонки
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition());
            int NRow = 6;
            for (int i = 0; i < NRow; i++)
                Grid.RowDefinitions.Add(new RowDefinition());

            // инициализировани объекты с датой
            DateStartPeriod = new DatePicker() {
                SelectedDate = currClient.DateLastPayment == default ? DateTime.Today : currClient.DateLastPayment};
            dateEndPeriod = new DatePicker() { };
            DateStartPeriod.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);
            dateEndPeriod.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);

            // инициализировали текстовые поля
            Label startLable = new Label() { Content = "Начало:" };
            Label endLable = new Label() { Content = "Конец:" };
            Label nameLable = new Label() { Content = "Продлить абонемент:" };
            Label chooseLable = new Label() { Content = "Выберите абонемент:" };
            Label costLable = new Label() { Content = "Укажите сумму оплаты:" };

            // инициализируем combo box для выбора абонемента
            comboBox = new ComboBox();
            comboBoxAnotherIndex = 5; // индекс поля "другое"
            comboBox.SelectedIndex = comboBoxAnotherIndex; // изначально выбран "другое"

            comboBox.SelectionChanged += new SelectionChangedEventHandler(ComboBox_SelectionChanged);
            string[] mounths = { "1 месяц", "2 месяца", "3 месяца", "6 месяцев", "1 год", "другое" };
            foreach (string content in mounths)
                comboBox.Items.Add(new Label() { Content = content });
            // инициализируем окно ввода суммы оплаты
           costTextBox = new() { Margin = new Thickness(0, 3, 3, 3) };
            // иницализируем кнопку продлить
            Button ButSave = new Button() { Content = "продлить", Margin = new Thickness(0, 3, 3, 3) };
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

            // устанавливаем название для указания цены
            Grid.SetColumn(costLable, 0);
            Grid.SetRow(costLable, 4);
            // устанавливаем текст бокс для указания цены
            Grid.SetColumn(costTextBox, 1);
            Grid.SetRow(costTextBox, 4);

            // устанавливаем кнопку продлить
            Grid.SetColumn(ButSave, 0);
            Grid.SetRow(ButSave, 5);

        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] multMounth = { 1, 2, 3, 6, 12 };
            int index = (sender as ComboBox).SelectedIndex;
            if (index != -1 && index != comboBoxAnotherIndex)
            {
                DateTime start = DateStartPeriod.DisplayDate;
                dateEndPeriod.SelectedDate = start.AddMonths(multMounth[index]);
            }

        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox.SelectedIndex = comboBoxAnotherIndex;
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
            float temp;
            if (!(float.TryParse(costTextBox.Text.Trim(' '), out temp)))
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
            if (currClient.DateLastPayment > DateStartPeriod.SelectedDate)
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
                DateStartPeriod.SelectedDate.Value, dateEndPeriod.SelectedDate.Value, float.Parse(costTextBox.Text)));
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

        }
        public void Show() => CurrWindow.spPayment.Children.Add(Grid);

    }
    
}
