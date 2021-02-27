using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ControlPanel.Exceptions;

namespace ControlPanel.Model
{
    class ClientModel
    {
        public string Surname { get; private set; }    
        public string Name { get; private set; }       
        public string Patronymic { get; private set; } 
        public DateTime BirthDate { get; private set; }
        public char[] PhoneNumber { get; private set; }//Длинна должна быть равна 11 (например 78005553535)

        private ClientModel() { }

        public ClientModel(string surname, string name, string patronymic,
                           DateTime BirthDate, char[] phoneNumber)
        {
            if (surname is null || name is null || patronymic is null || phoneNumber is null)
                throw new ArgumentNullException();

            if (phoneNumber.Length != 11)
                throw new PhoneNumberException("Номер имеет некорректную длину");

            Surname = surname;
            Name = name;
            Patronymic = patronymic;
            this.BirthDate = BirthDate;
            PhoneNumber = phoneNumber;
        }

    }
}
