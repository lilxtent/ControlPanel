using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlPanel.Services;
using ControlPanel.Model;
using ControlPanel.View;
using ControlPanel.Sourses;
using System.Windows.Threading;
using ControlPanel.ViewModel;

namespace ControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationContext DB { get; set; }
        private CameraModel camera { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DB = new ApplicationContext();
            camera = new CameraModel(DB);
        }


        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAllClientsShortData(lbClients);
        }

        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            var camWindow = new CamWindow(camera);
            camWindow.Show();
        }

        private void lbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbClients.SelectedIndex != -1)
            {
                var lbi = (ClientModelInfo)(lbClients.SelectedItem as ListBoxItem).Content;
                PersonalUnit[] personalObj = {
                new PersonalAvatar(lbi),
                new PersonalFIO(lbi),
                new PersonalPhone(lbi),
                new PersonalBirthDate(lbi),
                new PersonalButtonsLine(this)
                };
                spPersonalArea.Children.Clear();
                spPayment.Children.Clear();
                foreach (var el in personalObj)
                    spPersonalArea.Children.Add(el.getGrid());
            }
            else
            {
                spPersonalArea.Children.Clear();
                spPayment.Children.Clear();
            }
        }

        private void butAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow userWindow = new AddUserWindow();
            userWindow.Show();
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxSearch.Text is null || TextBoxSearch.Text.Trim(' ') is "")
            {
                ShowAllClientsShortData(lbClients);
            }
            else
            {
                lbClients.Items.Clear();
                foreach (ClientModel Client in DB.ClientsModels.ToList())
                {
                    if (Client.FIO.ToLower().Contains(TextBoxSearch.Text.Trim(' ').ToLower()))
                    {
                        lbClients.Items.Add(new ListBoxShortClientData(Client));
                    }
                }
            }
        }

        /// <summary>
        /// Очищает переданный ListBox и выводит в нем краткую информацию о всех клиентах
        /// </summary>
        private void ShowAllClientsShortData(ListBox Box)
        {
            Box.Items.Clear();
            foreach (ClientModel Client in DB.ClientsModels.ToList())
                Box.Items.Add(new ListBoxShortClientData(Client));
        }

        public void UpdateClientsList(ListBox Box)
        {
            DB = new ApplicationContext();
            ShowAllClientsShortData(Box);
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            UpdateClientsList(lbClients);
        }
    }
}
