using ControlPanel.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ControlPanel.ViewModel.MainWindow
{
    class TodayVisitsList : IEnumerable
    {
        public List<ShortVisitViewModel> Visits { get; set; }
        private Dictionary<int, string> ClientsFIO { get; set; }

        public TodayVisitsList() {
            ClientsFIO = new();
            Visits = new();
            ApplicationContext DB = new();
            foreach (var client in DB.ClientsModels)
                ClientsFIO.Add(client.ID, client.FIO);
            foreach (var visit in DB.Visits.Where(x => x.Date.Date == DateTime.Now.Date).OrderBy(x => x.Date))
                Visits.Add(new ShortVisitViewModel(ClientsFIO[visit.ID], visit.Date));
        }

        public void Add(ShortVisitViewModel visit) => Visits.Add(visit);

        public IEnumerator GetEnumerator() => Visits.GetEnumerator();
    }

    public class ShortVisitViewModel
    {
        public string ShortFIO { get; private set; }
        public string VisitTime { get; private set; }

        private ShortVisitViewModel() { }

        public ShortVisitViewModel(string FIO, DateTime VisitTime)
        {
            string[] fio = FIO.Split(' ');
            ShortFIO = $"{fio[0]} {fio[1][0]}. {fio[2][0]}.";
            this.VisitTime = VisitTime.TimeOfDay.ToString(@"hh\:mm\:ss");
        }
    }
}
