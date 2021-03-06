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
        public string PhotoPath { get; private set; }
        public string ParentType { get; private set; }
        public string ParentFIO { get; private set; }
        public string ParentPhoneNumber { get; private set; }//Длинна должна быть равна 11 (например 78005553535)
        public DateTime DateLastPayment { get; private set; }

        private ClientModel() { }

        public ClientModel(int id, string surname, string name, string patronymic,
                           DateTime birthDate, string phoneNumber, DateTime dateLastPayment = default(DateTime),
                           string photoPath=null, string parentType=null,
                           string parentFIO= null, string parentPhoneNumber=null)
        {
            if (surname is null || name is null || patronymic is null || phoneNumber is null)
                throw new ArgumentNullException();

            if (phoneNumber.Length != 11 || (!(parentPhoneNumber is null) && parentPhoneNumber.Length != 11))
                throw new PhoneNumberException("Номер имеет некорректную длину");
            ID = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            BirthDate = birthDate;
            PhoneNumber = phoneNumber;
            DateLastPayment = dateLastPayment;

            PhotoPath = photoPath is null ? @"..\..\..\Sourses\Images\default-user-image.png" : photoPath;
            ParentType = parentType;
            ParentFIO = parentFIO;
            ParentPhoneNumber = parentPhoneNumber;
        }
        public void SaveToDB(ApplicationContext DB)
        {
            DB.ClientsModels.Add(this);
        }
    }
    class ClientModelInfo
    {
        public ClientModel clientModel { get; private set; }
        public ClientModelInfo(ClientModel model)
        {
            clientModel = model;
        }

        public override string ToString()
        {
            return $"{clientModel.Surname} {clientModel.Name} " +
                $"{clientModel.Patronymic} телефон: {clientModel.PhoneNumber}";
        }

    } 
}
