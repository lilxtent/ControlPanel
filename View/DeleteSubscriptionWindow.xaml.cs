using ControlPanel.Services;
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
    /// Логика взаимодействия для DeleteSubscriptionWindow.xaml
    /// </summary>
    public partial class DeleteSubscriptionWindow : Window
    {
        public DeleteSubscriptionWindow()
        {
            InitializeComponent();
            // инициализируем список тренеров
            ApplicationContext DB = new();
            foreach (var subscription in DB.Subscriptions.ToList())
            {
                CbSubscription.Items.Add(new Label() { Content = subscription.Subscription });
            }
        }

        private bool CheckSubscription()
        {
            if (CbSubscription.SelectedIndex == -1)
            {
                MessageBox.Show("Укажите абонемент", "Ошибка");
                return false;
            }
            return true;
        }
       
        private void ButtonDeleteSubscription_Click(object sender, RoutedEventArgs e)
        {
            // проверяем пустое ли окно c абонементом
            if (!CheckSubscription())
                return;
            if (MessageBox.Show(this, "Вы уверенны, что хотите удалить абонемент? Будет удален навсегда. "
                , "Удадение абонемента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ApplicationContext DB = new();
                // удаляем группу тренера
                var SubscriptionForDelete = DB.Subscriptions.Where(x => x.Subscription == (CbSubscription.SelectedItem as Label).Content.ToString()).ToArray();
                DB.Subscriptions.RemoveRange(SubscriptionForDelete);
                DB.SaveChanges();
                this.Close();
            }
        }
    }
}
