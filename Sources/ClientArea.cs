using ControlPanel.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPanel.Sourses
{
    class ClientArea
    {
        private string Surname { get; set; }
        private string Name { get; set; }
        private string Patronymic { get; set; }
        private ClientModel Client { get; set; }
        private Grid GridData { get; set; }

        public ClientArea(ClientModel client)
        {
            Client = client;
            Surname = client.Surname;
            Name = client.Name;
            Patronymic = client.Patronymic;
            GridData = new();
            GridData.DataContext = client; // тут мы храним класс с информацией о клиенте
        }
        public Grid getGrid()
        {
            
            // разбиваем grid на колонки
            // первая колонка фиксированной длины
            GridData.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(280) });
            GridData.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(25) });

            int nColumn = 4;
            for (int i = 0; i < nColumn; i++)
                GridData.ColumnDefinitions.Add(new ColumnDefinition());
            Label[] LabelsForGrid = {
            CreateFIO(), CreateHeaderPhone(), CreatePhone(),
                CreateHeaderBalance(), CreateBalance()};

            // добавляем все записи
            foreach (Label obj in LabelsForGrid)
                GridData.Children.Add(obj);
            Separator Sep = CreateSeparator();
            GridData.Children.Add(Sep); // косметический сепаратор
            // раскидываем по сетке
            Grid.SetColumn(LabelsForGrid[0], 0);
            Grid.SetColumn(Sep, 1);
            Grid.SetColumn(LabelsForGrid[1], 2);
            Grid.SetColumn(LabelsForGrid[2], 3);
            Grid.SetColumn(LabelsForGrid[3], 4);
            Grid.SetColumn(LabelsForGrid[4], 5);

            return GridData;
        }
        private Label CreateFIO()
        {

            Label LocalData = (new Label()
            {
                Content = $"{Client.Surname} {Client.Name} {Client.Patronymic}",
                FontWeight = FontWeights.Bold
            });
            return LocalData;
        }
        private Separator CreateSeparator()
        {
            Separator LocalSeparator = new()
            {
                Height = 20,
                Width = 20,
                RenderTransform = new RotateTransform(90),
                Margin = new Thickness(0, 0, -50, 0)
        };
            return LocalSeparator;
        }
        private Label CreateHeaderPhone()
        {
            Label LocalData = (new Label()
            {
                Content = "Телефон:",
                FontWeight = FontWeights.Bold
            });
            return LocalData;
        }
        private Label CreatePhone()
        {
            string phone = Client.PhoneNumber;
            string phoneNumberView = phone[0] + "-" + phone[1..4] + "-" + phone[4..6] + "-" + phone[6..8] + "-" + phone[8..];
            Label LocalData = (new Label() { Content = $"{phoneNumberView}", FontSize = 12});
            return LocalData;
        }
        private Label CreateHeaderBalance()
        {
            Label LocalData = (new Label() { Content = "Баланс:", FontWeight = FontWeights.Bold});
            return LocalData;
        }
        private Label CreateBalance()
        {
            Label LocalData = (new Label() { Content = $"{RestOfDaysStr()}", Background = FindColor(Client),
            Width = 100, HorizontalAlignment = HorizontalAlignment.Right});
            return LocalData;
        }
        private SolidColorBrush FindColor(ClientModel ClientData)
        {
            SolidColorBrush ColorBrush = new SolidColorBrush(Color.FromArgb(200, 196, 196, 194));
            // в случае если клиент ни разу не производил оплату
            if (ClientData.DateLastPayment == default)
            {
                return ColorBrush;
            }

            int diffDays = (ClientData.DateLastPayment - DateTime.Today).Days;
            // абонемент просрочен
            if (diffDays < 0)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 255, 112, 99));
            }
            // остался 1 день
            else if (diffDays <= 1)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 250, 155, 100));
            }
            // осталось 3 дня
            else if (diffDays <= 3)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 242, 250, 100));
            }
            // осталось больше 3 дней
            else
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 206, 255, 107));
            }
            return ColorBrush;
        }
        private string RestOfDaysStr()
        {
            string answer = "не оплачено";
            if (Client.DateLastPayment == default)
                return answer;
            int diffDays = (Client.DateLastPayment - DateTime.Today).Days;
            if (diffDays < 0)
            {
                answer = $" просрочено {Math.Abs(diffDays)} день";
            }
            else if (diffDays < 30)
            {
                answer = $"{diffDays} день";
            }
            else
            {
                answer = $"более 30 дней";
            }
            return answer;
        }
    }
}
