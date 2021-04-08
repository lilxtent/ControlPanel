using ControlPanel.Model;
using ControlPanel.Services;
using System.Linq;
using System.Windows;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для AddTrainerWindow.xaml
    /// </summary>
    public partial class AddTrainerWindow : Window
    {
        private MainWindow OwnerWindow { get; set; }
        public AddTrainerWindow(MainWindow owner)
        {
            InitializeComponent();
            OwnerWindow = owner;
        }

        private void TrimAllTextBox()
        {
            TrainerSurname.Text = TrainerSurname.Text.Trim(' ');
            TrainerName.Text = TrainerName.Text.Trim(' ');
            TrainerPatronymic.Text = TrainerPatronymic.Text.Trim(' ');
        }
        private void ThisFieldCantBeEmpty(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть пустым: {fieldName}", "Ошибка");
        }
        private bool CheckAllTextBox()
        {
            TrimAllTextBox();

            if (TrainerSurname.Text is null || TrainerSurname.Text == "")
            {
                ThisFieldCantBeEmpty("Фамилия");
                return false;
            }
            if (TrainerName.Text is null || TrainerName.Text == "")
            {
                ThisFieldCantBeEmpty("Имя");
                return false;
            }
            if (TrainerPatronymic.Text is null || TrainerPatronymic.Text == "")
            {
                ThisFieldCantBeEmpty("Отчество");
                return false;
            }
            string shortFullname = $"{TrainerSurname.Text} {TrainerName.Text[0]}. {TrainerPatronymic.Text[0]}.";
            var TrainersList = (new ApplicationContext()).Trainers.ToList().Where(x => x.ShortFullname == shortFullname).ToArray();
            if (TrainersList.Length != 0)
            {
                MessageBox.Show("Тренер с такими инициалами уже записан!", "Ошибка");
                return false;
            }
            return true;
        }
        private void ButtonSaveTrainer_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAllTextBox()) return;
            string shortFullname = $"{TrainerSurname.Text} {TrainerName.Text[0]}. {TrainerPatronymic.Text[0]}.";
            TrainerModel Trainer = new(shortFullname, TrainerSurname.Text, TrainerName.Text, TrainerPatronymic.Text);
            ApplicationContext DB = new();
            DB.Trainers.Add(Trainer);
            DB.SaveChanges();
            OwnerWindow.UpdateTrainers(); // обновляем тренеров
            this.Close();
        }
    }
}
