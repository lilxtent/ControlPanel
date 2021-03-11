using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.Model;
using System.Windows.Media;
using ControlPanel.View;
using ControlPanel.Services;


namespace ControlPanel.Sourses
{
    abstract class PersonalUnit
    {
        public virtual Grid getGrid()
        {
            Grid grid = new Grid();
            grid.Children.Add(new Label() { Content = "пусто" });
            return grid;
        }
    }
    class PersonalFIO : PersonalUnit
    {
        private string Surname { get; set; }
        private string Name { get; set; }
        private string Patronymic { get; set; }

        public PersonalFIO(ClientModelInfo client)
        {
            Surname = client.clientModel.Surname;
            Name = client.clientModel.Name;
            Patronymic = client.clientModel.Patronymic;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.Children.Add(new Label() { Content = $"Спортсмен: {Surname} {Name} {Patronymic}" });

            return grid;
        }
    }

    class PersonalPhone : PersonalUnit
    {
        private string Phone { get; set; }
        public PersonalPhone(ClientModelInfo client)
        {
            Phone = client.clientModel.PhoneNumber;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.Children.Add(new Label() { Content = $"Телефон: {Phone}" });

            return grid;
        }
    }
    class PersonalBirthDate : PersonalUnit
    {
        private DateTime BirthDate { get; set; }
        public PersonalBirthDate(ClientModelInfo client)
        {
            BirthDate = client.clientModel.BirthDate;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.Children.Add(new Label() { Content = $"Дата рождения: {BirthDate.ToShortDateString()}" });

            return grid;
        }
    }
    class PersonalButton : PersonalUnit
    {
        private MainWindow window { get; set; }
        private DatePicker dateStartPeriod { get; set; }
        private DatePicker dateEndPeriod { get; set; }
        private ComboBox comboBox { get; set; }
        private string costTextMemder { get; set; }

        private int comboBoxAnotherIndex; 
        public PersonalButton(MainWindow mainWindow)
        {
            window = mainWindow;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Button butRegistr = new Button()
            {
                Content = $"редактировать",
                Margin = new Thickness(5, 0, 0, 0)
            };
            Button butBalance = new Button()
            {
                Content = $"продлить абонемент",
                Margin = new Thickness(0, 0, 5, 0),
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 244, 137)),
            };
            butBalance.Click += new RoutedEventHandler(butBalance_Click);

            grid.Children.Add(butRegistr);
            grid.Children.Add(butBalance);

            Grid.SetColumn(butBalance, 0);
            Grid.SetColumn(butRegistr, 1);


            return grid;
        }
        private void butBalance_Click(object sender, RoutedEventArgs e)
        {
            Grid grid = new Grid();
            // разметили сетку 5 строк на 2 колонки
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(140)});
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());

            // инициализировани объекты с датой
            dateStartPeriod = new DatePicker() { SelectedDate = DateTime.Today };
            dateEndPeriod = new DatePicker() { };
            dateStartPeriod.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(DatePicker_SelectedDateChanged);
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
            string[] mounths = { "1 месяц", "2 месяца", "3 месяца", "6 месяцев", "1 год", "другое"};
            foreach (string content in mounths)
                comboBox.Items.Add(new Label() { Content = content });
            // инициализируем окно ввода суммы оплаты
            TextBox costTextBox = new() { Margin = new Thickness(0, 3, 3, 3)};
            // иницализируем кнопку продлить
            Button butExtend = new Button() { Content = "продлить",  Margin = new Thickness(0, 3, 3, 3)  };
            butExtend.Click += new RoutedEventHandler(butExtend_Click);



            grid.Children.Add(nameLable);
            grid.Children.Add(startLable);
            grid.Children.Add(endLable);
            grid.Children.Add(dateStartPeriod);
            grid.Children.Add(dateEndPeriod);
            grid.Children.Add(comboBox);
            grid.Children.Add(chooseLable);
            grid.Children.Add(costLable);
            grid.Children.Add(costTextBox);

            grid.Children.Add(butExtend);



            // устанавливаем название
            Grid.SetColumn(nameLable, 0);
            Grid.SetRow(nameLable, 0);

            // устанавливаем надпись старта
            Grid.SetColumn(startLable, 0);
            Grid.SetRow(startLable, 1);
            // устанавливаем дату старта
            Grid.SetColumn(dateStartPeriod, 1);
            Grid.SetRow(dateStartPeriod, 1);

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
            Grid.SetColumn(butExtend, 0);
            Grid.SetRow(butExtend, 5);

            window.spPersonalArea.Children.Add(grid);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int[] multMounth = { 1, 2, 3, 6, 12 };
            int index = (sender as ComboBox).SelectedIndex;
            if (index != -1 && index != comboBoxAnotherIndex)
            {
                DateTime start = dateStartPeriod.DisplayDate;
                dateEndPeriod.SelectedDate = start.AddMonths(multMounth[index]);
            }

        }
        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            comboBox.SelectedIndex = comboBoxAnotherIndex;
        }
        private void butExtend_Click(object sender, RoutedEventArgs e)
        {
            ApplicationContext DB = new();
            ListBoxItem lbi = ((window.lbClients as ListBox).SelectedItem as ListBoxItem);
            var currClient = lbi.Content as ClientModelInfo;
            int id = currClient.clientModel.ID;
            DateTime timePayment = DateTime.Today;
            timePayment.AddHours(DateTime.Now.Date.Hour);

            DB.Payments.Add(new PaymentModel(id, DateTime.Now,
                dateStartPeriod.SelectedDate, dateEndPeriod.SelectedDate, 1200));
            DB.SaveChanges();
        }
        

    }
}
