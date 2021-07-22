using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.Model
{
    public class SubscriptionModel
    {
        [Key]
        public string Subscription { get; private set; }//Уникальное поле для базы данных
        public int Cost { get; private set; }

        private SubscriptionModel() { }

        public SubscriptionModel(string subscription, int cost)
        {
            Subscription = subscription;
            Cost = cost;
        }
    }
}
