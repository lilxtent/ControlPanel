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

namespace ControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ApplicationContext DB { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DB = new ApplicationContext();
            AddUserWindow test = new AddUserWindow();
            test.ShowDialog();
        }

        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            List<ClientModel> clients = DB.ClientsModels.ToList();
            ListBoxItem item;
            
            foreach (ClientModel client in clients)
            {
                string msg = $"{client.Surname} {client.Name} {client.PhoneNumber}";

                item = new ListBoxItem()
                {
                    Content = new ClientModelInfo(client), Margin = new Thickness(1),
                    BorderBrush = Brushes.Black, BorderThickness = new Thickness(1)
                };
                lbClients.Items.Add(item);
            }
        }
        private void lbClients_DisplayMember(object sender, EventArgs e)
        {

        }

        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            CamWindow camWindow = new CamWindow();
            camWindow.Show();
        }

        private void lbClients_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(lbClients.SelectedIndex.ToString());
        }

        private void lbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBoxItem lbi = ((sender as ListBox).SelectedItem as ListBoxItem);
            object[] personalObj = {
                new PersonalFIO((ClientModelInfo)lbi.Content),
                new PersonalPhone((ClientModelInfo)lbi.Content),
                new PersonalBirthDate((ClientModelInfo)lbi.Content),
                new PersonalButton()
            };
            spPersonalArea.Children.Clear();
            foreach (object el in personalObj)
                spPersonalArea.Children.Add(((PersonalUnit)el).getGrid());

        }
    }   
}
