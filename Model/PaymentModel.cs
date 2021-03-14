using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ControlPanel.Exceptions;
using ControlPanel.Services;

namespace ControlPanel.Model
{
    public class PaymentModel
    {
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
        public void SaveToDB(ApplicationContext DB)
        {
            DB.Payments.Add(this);
        }
    }
}
