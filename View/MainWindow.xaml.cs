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
            DB.ClientsModels.Add(new ClientModel(100458, "Латкин", "Борис", "Курапович",
                new DateTime(2012, 12, 12), "79137564745", new DateTime(2021, 03, 05)));
            DB.SaveChanges();
        }

        
        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            List<ClientModel> clients = DB.ClientsModels.ToList();

            foreach (ClientModel client in clients)
            {
                string msg = $"{client.Surname} {client.Name} {client.PhoneNumber}";
                lbClients.Items.Add(msg);
            }
        }

        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            CamWindow camWindow = new CamWindow();
            camWindow.Show();

        }
    }
}
