using System;
using System.ComponentModel.DataAnnotations;

namespace ControlPanel.Model
{
    public class VisitModel
    {
        [Key]
        public int UniqueID { get; private set; }//Уникальное поле для базы данных
        public int ID { get; private set; }
        public DateTime Date { get; private set; }

        private VisitModel() { }

        public VisitModel(int id, DateTime DateVisit)
        {
            ID = id;
            Date = DateVisit;
        }

        /// <summary>
        /// Использовать только при обновлении ID у клиента
        /// </summary>
        public VisitModel(VisitModel ModelWithOldID, int newID) : this(newID, ModelWithOldID.Date)
        {
            UniqueID = ModelWithOldID.UniqueID;
        }
    }
}
