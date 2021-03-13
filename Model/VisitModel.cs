using ControlPanel.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.Model
{
    public class VisitModel
    {
        public int ID { get; private set; }
        public DateTime Date { get; private set; }

        private VisitModel() { }

        public VisitModel(int id, DateTime DateVisit)
        {

            ID = id;
            Date = DateVisit;
        }
    }
}
