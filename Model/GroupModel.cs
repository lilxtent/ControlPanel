using System.ComponentModel.DataAnnotations;

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
