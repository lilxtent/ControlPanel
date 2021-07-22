using ControlPanel.Model;
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
    /// Логика взаимодействия для AddSubscriptionWindow.xaml
    /// </summary>
    public partial class AddSubscriptionWindow : Window
    {
        private MainWindow OwnerWindow { get; set; }
        public AddSubscriptionWindow(MainWindow owner)
        {
            InitializeComponent();
            OwnerWindow = owner;
        }
        private void TrimAllTextBox()
        {
            SubscriptoinTb.Text = SubscriptoinTb.Text.Trim(' ');
            CostTb.Text = CostTb.Text.Trim(' ');
        }
        private void ThisFieldCantBeEmpty(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть пустым: {fieldName}", "Ошибка");
        }
        private void ThisFieldCantBeString(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть не целочисленным или пустым: {fieldName}", "Ошибка");
        }
        private bool CheckAllTextBox()
        {
            TrimAllTextBox();

            if (SubscriptoinTb.Text is null || SubscriptoinTb.Text == "")
            {
                ThisFieldCantBeEmpty("Абонемент");
                return false;
            }
            int num;
            if (CostTb.Text is null || Int32.TryParse(CostTb.Text, out num) == false)
            {
                ThisFieldCantBeString("Цена");
                return false;
            }
            
            var TrainersList = (new ApplicationContext()).Subscriptions.ToList().Where(x => x.Subscription == SubscriptoinTb.Text).ToArray();
            if (TrainersList.Length != 0)
            {
                MessageBox.Show("Абонемент с таким названием уже есть!", "Ошибка");
                return false;
            }
            return true;
        }
        private void ButtonSubscriptoin_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAllTextBox()) return;
            SubscriptionModel Subscriptions = new(SubscriptoinTb.Text, int.Parse(CostTb.Text));
            ApplicationContext DB = new();
            DB.Subscriptions.Add(Subscriptions);
            DB.SaveChanges();
            OwnerWindow.UpdateTrainers(); // обновляем тренеров
            this.Close();
        }
    }
}
