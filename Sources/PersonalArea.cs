using ControlPanel.Model;
using System;
using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

            GridData.Children.Add(new Label()
            {
                Content = $"{Surname} {Name} {Patronymic}",
                FontWeight = FontWeights.Bold,
            });

            return GridData;
        }
    }
    class PersonalTrainer : PersonalUnit
    {
        private string Trainer { get; set; }

        public PersonalTrainer(ClientModel client)
        {
            Trainer = client.Trainer;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();

            GridData.Children.Add(new Label()
            {
                Content = $"Тренер: {Trainer}",
            });

            return GridData;
        }
    }
    class PersonalGroup : PersonalUnit
    {
        private string Group { get; set; }

        public PersonalGroup(ClientModel client)
        {
            Group = client.Group;
        }
        public override Grid getGrid()
        {
            Grid GridData = new Grid();

            GridData.Children.Add(new Label()
            {
                Content = $"Группа: {Group}",
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
            Uri UriToProfileImage;
            if (File.Exists(ImagePath))
            {
                UriToProfileImage = new Uri(Path.GetFullPath(ImagePath));
            }
            else
            {
                string pathToDefaultPhoto = ConfigurationManager.AppSettings["DefaultPhotoPath"].ToString();
                UriToProfileImage = new Uri(Path.GetFullPath(pathToDefaultPhoto));
            }

            Image ImageContainer = new()
            {
                Width = 140,
                Height = 120,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(4, 4, 0, 0),
                Source = new BitmapImage(UriToProfileImage)
            };

            Grid GridData = new();
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
            GridData.Children.Add(new Label() { Content = $"Телефон: {ConvPhone()}" });

            return GridData;
        }

        private string ConvPhone() => $"{Phone[0]}-{Phone[1..4]}-{Phone[4..6]}-{Phone[6..8]}-{Phone[8..]}";
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
