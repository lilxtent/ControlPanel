using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControlPanel.Model
{
    public class GroupModel
    {
        [Key]
        public int Key { get; set; } //Уникальное поле для базы данных
        public string Trainer { get; set; }
        public string Group { get; set; }

        public GroupModel(string trainer, string group)
        {
            Trainer = trainer;
            Group = group;
        }
    }
}
