using ControlPanel.Model;
using System.Windows;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для PaymentJournal.xaml
    /// </summary>
    public partial class PaymentJournalWindow : Window
    {

        private PaymentJournalWindow()
        {
            InitializeComponent();
        }

        public PaymentJournalWindow(ClientModel Client)
        {
            InitializeComponent();
            Title = $"Журнал оплаты {Client.FIO}";
            PaymentJournalTable.ItemsSource = Client.GetPayments();
        }
    }
}