using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.Model;
using System.Windows.Media;
using ControlPanel.View;
using ControlPanel.Services;
using System.Windows.Media.Imaging;
using ControlPanel.ViewModel.MainWindow;

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

        public PersonalFIO(ClientModel client)
        {
            Surname = client.Surname;
            Name = client.Name;
            Patronymic = client.Patronymic;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();

            GridData.Children.Add(new Label() { Content = $"{Surname} {Name} {Patronymic}",
                FontWeight = FontWeights.Bold,
            });

            return GridData;
        }
    }

    class PersonalAvatar : PersonalUnit
    {
        private string ImagePath { get; set; }

        public PersonalAvatar(ClientModel client)
        {
            ImagePath = client.PhotoPath;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid() {};
            Image ImageContainer = new Image()
            {
                Width = 140,
                Height = 120,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(4, 4, 0, 0)
            };
            ImageSource Image;
            BitmapImage MapImage = new BitmapImage(new Uri(@"Images\default-user-image.png", UriKind.Relative));
            try
            {
                MapImage = new BitmapImage(new Uri(ImagePath, UriKind.Absolute));
            }
            catch (System.IO.FileNotFoundException)
            {
                MapImage = new BitmapImage(new Uri("pack://application:,,,/View/default-user-image.png"));
            }
            Image = MapImage;
            ImageContainer.Source = Image;
            GridData.Children.Add(ImageContainer);

            return GridData;
        }
    }
    class PersonalPhone : PersonalUnit
    {
        private string Phone { get; set; }
        public PersonalPhone(ClientModel client)
        {
            Phone = client.PhoneNumber;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();
            GridData.Children.Add(new Label() { Content = $"Телефон: {Phone}" });

            return GridData;
        }
    }
    class PersonalSection : PersonalUnit
    {
        private string section { get; set; }
        public PersonalSection(ClientModel client)
        {
            section = client.Section;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();
            if (section == default) section = "не указано";
            GridData.Children.Add(new Label() { Content = $"Секция: {section}" });

            return GridData;
        }
    }

    class PersonalBirthDate : PersonalUnit
    {
        private DateTime BirthDate { get; set; }
        public PersonalBirthDate(ClientModel client)
        {
            BirthDate = client.BirthDate;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();
            GridData.Children.Add(new Label() { Content = $"Дата рождения: {BirthDate.ToShortDateString()}" });

            return GridData;
        }
    }
    class PersonalLastPay : PersonalUnit
    {
        private DateTime LastPay { get; set; }
        public PersonalLastPay(ClientModel client)
        {
            LastPay = client.DateLastPayment;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();
            string lastPay = "нет оплат";
            if (LastPay != default)
                lastPay = LastPay.ToLongDateString();

            GridData.Children.Add(new Label() { Content = $"Оплачено до: {lastPay}" });

            return GridData;
        }
    }
    class PersonalLastVisit : PersonalUnit
    {
        private DateTime LastVisit { get; set; }
        public PersonalLastVisit(ClientModel client)
        {
            LastVisit = client.DateLastVisit;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();
            string lastPay = "нет посещеинй";
            if (LastVisit != default)
                lastPay = LastVisit.ToLongDateString();

            GridData.Children.Add(new Label() { Content = $"Последний раз был: {lastPay}" });

            return GridData;
        }
    }
}
