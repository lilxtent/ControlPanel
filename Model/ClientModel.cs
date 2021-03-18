using ControlPanel.Exceptions;
using ControlPanel.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
        public DateTime DateLastVisit { get; set; }

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

        public void SaveToDB(ApplicationContext DB) => DB.ClientsModels.Add(this);

        public void RemoveFromDB(ApplicationContext DB)
        {
            DB.ClientsModels.Remove(this);
            DB.Payments.RemoveRange(GetPayments());
            DB.Visits.RemoveRange(GetVisits());
        }

        public void UpdateIDInDB(ApplicationContext DB, int newID)
        {
            DB.ClientsModels.Update(this);
            foreach (var payment in GetPayments())
                DB.Payments.Update(new PaymentModel(payment, newID));
            foreach (var visit in GetVisits())
                DB.Visits.Update(new VisitModel(visit, newID));
        }

        public IEnumerable<PaymentModel> GetPayments() =>
            (new ApplicationContext()).Payments.ToList().Where(x => x.ID == ID);

        public IEnumerable<VisitModel> GetVisits() =>
            (new ApplicationContext()).Visits.ToList().Where(x => x.ID == ID);

        public static IComparer SortOderAlphabetIncrease() => new SortOderAlphabetIncreaseHelper();
        public static IComparer SortOderAlphabetDecrease() => new SortOderAlphabetDecreaseHelper();
        public static IComparer SortOderDateLastPaymentNewer() => new SortOderDateLastPaymentNewerHelper();
        public static IComparer SortOderDateLastPaymentOlder() => new SortOderDateLastPaymentOlderHelper();

        private class SortOderAlphabetIncreaseHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                ClientModel cl1 = a as ClientModel;
                ClientModel cl2 = b as ClientModel;

                int shortest = cl1.Surname.Length > cl2.Surname.Length ?
                                               cl2.Surname.Length : cl1.Surname.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Surname[i] < cl2.Surname[i])
                        return -1;
                    else if (cl1.Surname[i] > cl2.Surname[i])
                        return 1;
                }

                shortest = cl1.Name.Length > cl2.Name.Length ?
                                                       cl2.Name.Length : cl1.Name.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Name[i] < cl2.Name[i])
                        return -1;
                    else if (cl1.Name[i] > cl2.Name[i])
                        return 1;
                }

                shortest = cl1.Patronymic.Length > cl2.Patronymic.Length ?
                                                       cl2.Patronymic.Length : cl1.Patronymic.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Patronymic[i] < cl2.Patronymic[i])
                        return -1;
                    else if (cl1.Patronymic[i] > cl2.Patronymic[i])
                        return 1;
                }

                return 0;
            }
        }

        private class SortOderAlphabetDecreaseHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                ClientModel cl1 = a as ClientModel;
                ClientModel cl2 = b as ClientModel;

                int shortest = cl1.Surname.Length > cl2.Surname.Length ?
                                               cl2.Surname.Length : cl1.Surname.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Surname[i] < cl2.Surname[i])
                        return 1;
                    else if (cl1.Surname[i] > cl2.Surname[i])
                        return -1;
                }

                shortest = cl1.Name.Length > cl2.Name.Length ?
                                                       cl2.Name.Length : cl1.Name.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Name[i] < cl2.Name[i])
                        return 1;
                    else if (cl1.Name[i] > cl2.Name[i])
                        return -1;
                }

                shortest = cl1.Patronymic.Length > cl2.Patronymic.Length ?
                                                       cl2.Patronymic.Length : cl1.Patronymic.Length;

                for (int i = 0; i < shortest; i++)
                {
                    if (cl1.Patronymic[i] < cl2.Patronymic[i])
                        return 1;
                    else if (cl1.Patronymic[i] > cl2.Patronymic[i])
                        return -1;
                }

                return 0;
            }
        }

        private class SortOderDateLastPaymentNewerHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                ClientModel cl1 = a as ClientModel;
                ClientModel cl2 = b as ClientModel;

                if (cl1.DateLastPayment > cl2.DateLastPayment)
                    return -1;
                else if (cl1.DateLastPayment < cl2.DateLastPayment)
                    return 1;
                else
                    return 0;
            }
        }

        private class SortOderDateLastPaymentOlderHelper : IComparer
        {
            int IComparer.Compare(object a, object b)
            {
                ClientModel cl1 = a as ClientModel;
                ClientModel cl2 = b as ClientModel;

                if (cl1.DateLastPayment > cl2.DateLastPayment)
                    return 1;
                else if (cl1.DateLastPayment < cl2.DateLastPayment)
                    return -1;
                else
                    return 0;
            }
        }
    }

}
