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
        public float Cost { get; private set; }

        private PaymentModel() { }

        public PaymentModel(int id, DateTime datePayment, DateTime startPeriod,
                           DateTime endPeriod, float cost)
        {
            ID = id;
            DatePayment = datePayment;
            StartPeriod = startPeriod;
            EndPeriod = endPeriod;
            Cost = cost;
        }

        /// <summary>
        /// Использовать только при обновлении ID у клиента
        /// </summary>
        public PaymentModel(PaymentModel ModelWithOldID, int newID) :
            this(newID,
            ModelWithOldID.DatePayment, ModelWithOldID.StartPeriod, ModelWithOldID.EndPeriod, ModelWithOldID.Cost)
        {
            UniqueID = ModelWithOldID.UniqueID;
        }

        public void SaveToDB(ApplicationContext DB)
        {
            DB.Payments.Add(this);
        }
    }
}
