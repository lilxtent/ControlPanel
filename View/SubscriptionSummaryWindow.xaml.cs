﻿using ControlPanel.Model;
using ControlPanel.Services;
using System;
using System.Linq;
using System.Windows;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для SubscriptionSummaryWindow.xaml
    /// </summary>
    public partial class SubscriptionSummaryWindow : Window
    {
        public SubscriptionSummaryWindow()
        {
            InitializeComponent();

            var DB = new ApplicationContext();

            SubscriptionSummaryTable.ItemsSource =
                DB.Payments.Where(x => x.EndPeriod > DateTime.Now)
                           .AsEnumerable()
                           .GroupBy(x => x.Subscription)
                           .Select(x => new TableRowData(x.ToArray()));

        }

        private class TableRowData
        { 
            public string Subscription { get; private set; }
            public int CostPerMonth { get; private set; }
            public int AmountOfClients { get; private set; }
            public int ProfitPerMonth { get; private set; }

            public TableRowData(PaymentModel[] paymentsWithSameSubscrition)
            {
                if (paymentsWithSameSubscrition.Length < 1)
                    throw new ArgumentException("Был пердан пустой массив");

                Subscription = paymentsWithSameSubscrition[0].Subscription;
                CostPerMonth = paymentsWithSameSubscrition[0].CostPerMonth;
                AmountOfClients = paymentsWithSameSubscrition.Length;
                ProfitPerMonth = paymentsWithSameSubscrition.Sum(x => x.CostPerMonth);
            }
        }
    }
}
