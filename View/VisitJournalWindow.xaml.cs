using ControlPanel.Model;
using System.Windows;

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
