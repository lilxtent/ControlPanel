using ControlPanel.Services;
using System.Windows;
using System.Linq;
using System;
using System.Collections.Generic;
using ControlPanel.Model;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для ValidSubscriptionsWindow.xaml
    /// </summary>
    public partial class ValidSubscriptionsWindow : Window
    {
        public ValidSubscriptionsWindow()
        {
            InitializeComponent();

            var DB = new ApplicationContext();

            var validPayments = 
                DB.Payments.Where(x => x.EndPeriod > DateTime.Now).ToList();
            var clientsWithValidSubscription = new List<ClientModel>();

            foreach (var payment in validPayments)
                clientsWithValidSubscription.Add(DB.ClientsModels.FirstOrDefault(x => x.ID == payment.ID));

            var tableData = new TableData[clientsWithValidSubscription.Count];

            for (int i = 0; i < tableData.Length; i++)
                tableData[i] = new TableData(clientsWithValidSubscription[i].FIO,
                                             validPayments[i]);

            ValidSubscriptionsTable.ItemsSource = tableData;
        }

        private class TableData
        {
            public string ClientsName { get; }
            public string SubscriptionType { get; }
            public int SubscriptionCost { get; }
            public DateTime SubscriptionExpireDate { get; }

            public TableData(string clientsName, PaymentModel paymentInfo)
            {
                ClientsName = clientsName;
                SubscriptionType = paymentInfo.Subscription;
                SubscriptionCost = paymentInfo.CostPerMonth;
                SubscriptionExpireDate = paymentInfo.EndPeriod;
            }
        }
    }
}
