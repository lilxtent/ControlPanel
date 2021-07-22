using ControlPanel.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace ControlPanel.Model
{
    public class PaymentModel
    {
        [Key]
        public int UniqueID { get; private set; }//Уникальное поле для базы данных
        public int ID { get; private set; }
        public DateTime DatePayment { get; private set; }
        public DateTime StartPeriod { get; private set; }
        public DateTime EndPeriod { get; private set; }
        public int Cost { get; private set; }
        public string Subscription { get; private set; }
        public int CostPerMonth { get; private set; }

        private PaymentModel() { }

        public PaymentModel(int id, DateTime datePayment, DateTime startPeriod,
                           DateTime endPeriod, int cost, string subscription, int costPerMonth)
        {
            ID = id;
            DatePayment = datePayment;
            StartPeriod = startPeriod;
            EndPeriod = endPeriod;
            Cost = cost;
            Subscription = subscription;
            CostPerMonth = costPerMonth;
        }

        /// <summary>
        /// Использовать только при обновлении ID у клиента
        /// </summary>
        public PaymentModel(PaymentModel ModelWithOldID, int newID) :
            this(newID,
            ModelWithOldID.DatePayment, ModelWithOldID.StartPeriod, ModelWithOldID.EndPeriod, ModelWithOldID.Cost,
            ModelWithOldID.Subscription, ModelWithOldID.CostPerMonth)
        {
            UniqueID = ModelWithOldID.UniqueID;
        }

        public void SaveToDB(ApplicationContext DB)
        {
            DB.Payments.Add(this);
        }
    }
}
