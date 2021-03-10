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
        private CameraModel camera { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DB = new ApplicationContext();
            camera = new CameraModel();
        }

        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (ClientModel client in DB.ClientsModels.ToList())
            {
                lbClients.Items.Add(new ListBoxItem()
                {
                    Content = new ClientModelInfo(client),
                    Margin = new Thickness(1),
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1)
                });
            }
        }

        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            var camWindow = new CamWindow(camera);
            camWindow.Show();
        }

        private void lbClients_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(lbClients.SelectedIndex.ToString());
        }

        private void lbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lbi = (ClientModelInfo)((sender as ListBox).SelectedItem as ListBoxItem).Content;
            PersonalUnit[] personalObj = {
                new PersonalFIO(lbi),
                new PersonalPhone(lbi),
                new PersonalBirthDate(lbi),
                new PersonalButton()
            };
            spPersonalArea.Children.Clear();
            foreach (var el in personalObj)
                spPersonalArea.Children.Add(el.getGrid());
        }

        private void butAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow userWindow = new AddUserWindow();
            userWindow.Show();
        }
    }
}
