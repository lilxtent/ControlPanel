using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlPanel.Exceptions;

namespace ControlPanel.Model
{
    class ClientModel
    {
        public int ID { get; private set; }
        public string Surname { get; private set; }    
        public string Name { get; private set; }       
        public string Patronymic { get; private set; } 
        public DateTime BirthDate { get; private set; }
        public string PhoneNumber { get; private set; }//Длинна должна быть равна 11 (например 78005553535)
        public string PhotoPath { get; private set; }

        private ClientModel() { }

        public ClientModel(int id, string surname, string name, string patronymic,
                           DateTime BirthDate, string phoneNumber, string photoPath)
        {
            if (surname is null || name is null || patronymic is null || phoneNumber is null || photoPath is null)
                throw new ArgumentNullException();

            if (phoneNumber.Length != 11)
                throw new PhoneNumberException("Номер имеет некорректную длину");
            ID = id;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            this.BirthDate = BirthDate;
            PhoneNumber = phoneNumber;
            PhotoPath = photoPath;
        }

    }
}
