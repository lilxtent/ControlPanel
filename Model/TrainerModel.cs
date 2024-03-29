﻿using System.ComponentModel.DataAnnotations;

namespace ControlPanel.Model
{
    public class TrainerModel
    {
        [Key]
        public string ShortFullname { get; private set; }//Уникальное поле для базы данных
        public string Surname { get; private set; }
        public string Name { get; private set; }
        public string Patronymic { get; private set; }

        private TrainerModel() { }

        public TrainerModel(string shortFullname, string surname, string name, string patronymic)
        {
            ShortFullname = shortFullname;
            Surname = surname;
            Name = name;
            Patronymic = patronymic;
        }
    }
}
