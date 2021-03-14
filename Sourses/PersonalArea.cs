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
    class PersonalButtonsLine : PersonalUnit
    {
        private MainWindow Window { get; set; }
        private bool isEnableAreaExtendSubscripion;
        public PersonalButtonsLine(MainWindow mainWindow)
        {
            Window = mainWindow;
            isEnableAreaExtendSubscripion = false;
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Button ButRegistr = new Button()
            {
                Content = $"редактировать",
                Margin = new Thickness(5, 0, 0, 0)
            };
            ButRegistr.Click += new RoutedEventHandler(ButRegistr_Click);

            Button ButExtendSubscription = new Button()
            {
                Content = $"продлить абонемент",
                Margin = new Thickness(0, 0, 5, 0),
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 244, 137)),
            };
            ButExtendSubscription.Click += new RoutedEventHandler(ButExtendSubscription_Click);

            grid.Children.Add(ButRegistr);
            grid.Children.Add(ButExtendSubscription);

            Grid.SetColumn(ButExtendSubscription, 0);
            Grid.SetColumn(ButRegistr, 1);


            return grid;
        }
        private void ButExtendSubscription_Click(object sender, RoutedEventArgs e)
        {
            // если мы уже активировали поле для продления абонемента мы его закроем и наоборот
            if (isEnableAreaExtendSubscripion)
            {
                Window.spPayment.Children.Clear();
                isEnableAreaExtendSubscripion = false;
            }
            else
            {
                ExtendSubscription AreaExtendSubscripion = new ExtendSubscription(Window);
                AreaExtendSubscripion.Show();
                isEnableAreaExtendSubscripion = true;
            }
        }
        private void ButRegistr_Click(object sender, RoutedEventArgs e)
        {
            // инициализируем окно редактирования
            EditClientProfile Editor = new EditClientProfile(((ClientModelInfo)(Window.lbClients.SelectedItem as ListBoxItem).Content).clientModel);
            Editor.Show();
        }
    }
}
