using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using ControlPanel.Model;
using System.Windows.Media;

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
        public PersonalButton()
        {
        }
        public override Grid getGrid()
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Button butRegistr = new Button() { Content = $"редактировать",
                Margin=new Thickness(5, 0, 0, 0)};
            Button butBalance = new Button()
            {
                Content = $"продлить абонемент",
                Margin = new Thickness(0, 0, 5, 0),
                Background = new SolidColorBrush(Color.FromArgb(255, 0, 244, 137))
        };

            grid.Children.Add(butRegistr);
            grid.Children.Add(butBalance);

            Grid.SetColumn(butBalance, 0);
            Grid.SetColumn(butRegistr, 1);


            return grid;
        }
    }
}
