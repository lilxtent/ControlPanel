using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlPanel.Exceptions;
using ControlPanel.Services;

namespace ControlPanel.Model
{
    public class ClientModel
    {
        public int ID { get; private set; }
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string Patronymic { get; private set; }
        public DateTime BirthDate { get; private set; }
        public string PhoneNumber { get; private set; }//Длинна должна быть равна 11 (например 78005553535)
        public DateTime DateLastPayment { get; private set; }
        public string Section { get; private set; }
        public string PhotoPath { get; private set; }
        public string ParentType { get; private set; }
        public string ParentFIO { get; private set; }
        public string ParentPhoneNumber { get; private set; }//Длинна должна быть равна 11 (например 78005553535)
        public DateTime DateLastVisit { get; private set; }//Длинна должна быть равна 11 (например 78005553535)


        public string FIO
        {
            get => Surname + " " + Name + " " + Patronymic;
        }

        private ClientModel() { }

        public ClientModel(int id, string surname, string name, string patronymic,
                           DateTime birthDate, string phoneNumber,
                           DateTime dateLastPayment = default(DateTime),
                           string section = default,
                           string photoPath = null, string parentType = null,
                           string parentFIO = null, string parentPhoneNumber = null, DateTime dateLastVisit = default(DateTime))
        {
            if (surname is null || name is null || patronymic is null || phoneNumber is null)
                throw new ArgumentNullException();

            if (phoneNumber.Length != 11 || ((parentPhoneNumber is not null && parentPhoneNumber != "") && parentPhoneNumber.Length != 11))
                throw new PhoneNumberException("Номер имеет некорректную длину");
            ID = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
            DateLastPayment = dateLastPayment;
            Section = section;
            PhotoPath = photoPath is null ? @"..\..\..\Sourses\Images\default-user-image.png" : photoPath;
            ParentType = parentType;
            ParentFIO = parentFIO;
            ParentPhoneNumber = parentPhoneNumber;
            DateLastVisit = dateLastVisit;
        }
        public void SetDateLastPayment(DateTime? Date) => DateLastPayment = (DateTime)Date;
        public void SaveToDB(ApplicationContext DB)
        {
            DB.ClientsModels.Add(this);
        }

        public IEnumerable<PaymentModel> GetPayments()
        {
            ApplicationContext DB = new();
            return DB.Payments.ToList().Where(x => x.ID == ID);
        }

        public IEnumerable<VisitModel> GetVisits()
        {
            ApplicationContext DB = new();
            return DB.Visits.ToList().Where(x => x.ID == ID);
        }
    }

    class ClientModelInfo
    {
        public ClientModel clientModel { get; private set; }
        public ClientModelInfo(ClientModel model)
        {
            clientModel = model;
        }
        private string RestOfDaysStr()
        {
            string answer = "не оплачено";
            if (clientModel.DateLastPayment == default)
                return answer;
            int diffDays = (clientModel.DateLastPayment - DateTime.Today).Days;
            if (diffDays < 0)
            {
                answer = $" просрочено {Math.Abs(diffDays)} день";
            }
            else if (diffDays < 30)
            {
                answer = $"{diffDays} день";
            }
            else
            {
                answer = $"более 30 дней";
            }
            return answer;
        }
        public override string ToString()
        {
            return $"{clientModel.Surname} {clientModel.Name} " +
                $"{clientModel.Patronymic} телефон: {clientModel.PhoneNumber} оплата: {RestOfDaysStr()}";
        }

    }
}
