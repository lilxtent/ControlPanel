using ControlPanel.Model;
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
using System.Windows.Shapes;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для VisitJournalWindow.xaml
    /// </summary>
    public partial class VisitJournalWindow : Window
    {
        private VisitJournalWindow()
        {
            InitializeComponent();
        }

        public VisitJournalWindow(ClientModel Client)
        {
            InitializeComponent();
            Title = $"Журанл посещений {Client.FIO}";
            VisitJournalTable.ItemsSource = Client.GetVisits();
        }
    }
}
