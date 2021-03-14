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

        public PersonalFIO(ClientModelInfo client)
        {
            Surname = client.clientModel.Surname;
            Name = client.clientModel.Name;
            Patronymic = client.clientModel.Patronymic;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.Children.Add(new Label() { Content = $"{Surname} {Name} {Patronymic}",
                FontWeight = FontWeights.Bold,
            });

            return grid;
        }
    }

    class PersonalAvatar : PersonalUnit
    {
        private string ImagePath { get; set; }

        public PersonalAvatar(ClientModelInfo client)
        {
            ImagePath = client.clientModel.PhotoPath;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid() {};
            Image ImageContainer = new Image()
            {
                Width = 140,
                Height = 120,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(4, 4, 0, 0)
            };
            ImageSource image;
            if (ImagePath is null)
                image = new BitmapImage(new Uri(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Sourses\Images\default-user-image.png", UriKind.Absolute));
            else
                image = new BitmapImage(new Uri(ImagePath, UriKind.Absolute));

            ImageContainer.Source = image;
            grid.Children.Add(ImageContainer);

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
    class PersonalSection : PersonalUnit
    {
        private string section { get; set; }
        public PersonalSection(ClientModelInfo client)
        {
            section = client.clientModel.Section;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            if (section == default) section = "не указано";
            grid.Children.Add(new Label() { Content = $"Секция: {section}" });

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
    class PersonalLastPay : PersonalUnit
    {
        private DateTime LastPay { get; set; }
        public PersonalLastPay(ClientModelInfo client)
        {
            LastPay = client.clientModel.DateLastPayment;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            string lastPay = "нет оплат";
            if (LastPay != default)
                lastPay = LastPay.ToLongDateString();

            grid.Children.Add(new Label() { Content = $"Оплачено до: {lastPay}" });

            return grid;
        }
    }
    class PersonalLastVisit : PersonalUnit
    {
        private DateTime LastVisit { get; set; }
        public PersonalLastVisit(ClientModelInfo client)
        {
            LastVisit = client.clientModel.DateLastVisit;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            string lastPay = "нет посещеинй";
            if (LastVisit != default)
                lastPay = LastVisit.ToLongDateString();

            grid.Children.Add(new Label() { Content = $"Последний раз был: {lastPay}" });

            return grid;
        }
    }
}
