using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlPanel.Model;
using ControlPanel.Services;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для DeleteTrainerWindow.xaml
    /// </summary>
    public partial class DeleteTrainerWindow : Window
    {
        public MainWindow OwnerWindow { get; private set; }
        public DeleteTrainerWindow(MainWindow owner)
        {
            OwnerWindow = owner;
            InitializeComponent();
            // инициализируем список тренеров
            ApplicationContext DB = new();
            foreach (var trainer in DB.Trainers.ToList())
            {
                CbTrainers.Items.Add(new Label() { Content = trainer.ShortFullname });
            }
        }

        private void CbTrainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CbGroups.Items.Clear();
            ApplicationContext DB = new ApplicationContext();
            foreach (GroupModel group in DB.Groups?.ToList().Where(x => x.Trainer == (CbTrainers.SelectedItem as Label).Content.ToString()))
                CbGroups.Items.Add(new Label() { Content = group.Group });
        }

        private bool CheckTrainer()
        {
            if (CbTrainers.SelectedIndex == -1)
            {
                MessageBox.Show("Укажите тренера", "Ошибка");
                return false;
            }
            return true;
        }
        private bool CheckGroup()
        {
            if (CbGroups.SelectedIndex == -1)
            {
                MessageBox.Show("Укажите группу", "Ошибка");
                return false;
            }
            return true;
        }
        private void ButtonDeleteTrainer_Click(object sender, RoutedEventArgs e)
        {
            // проверяем пустое ли окно с тренером
            if (!CheckTrainer())
                return;
            if (MessageBox.Show(this, "Вы уверенны, что хотите удалить тренера? Все группы принадлежащие этому тренеру также " +
                "будут удалены", "Удадение тренера", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            { 
                ApplicationContext DB = new();
                // удаляем самого тренера
                var TrainerForDelete = DB.Trainers.Where(x => x.ShortFullname == (CbTrainers.SelectedItem as Label).Content.ToString()).ToArray();
                DB.Trainers.RemoveRange(TrainerForDelete);
                // удаляем все группы этого тренера
                var GroupsForDelete = DB.Groups.Where(x => x.Trainer == (CbTrainers.SelectedItem as Label).Content.ToString()).ToArray();
                DB.Groups.RemoveRange(GroupsForDelete);
                DB.SaveChanges();
                OwnerWindow.UpdateTrainers();
                this.Close();
            }
        }
        private void ButtonDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            // проверяем пустое ли окно с тренером
            if (!CheckTrainer())
                return;
            // проверяем пустое ли окно с группой
            if (!CheckGroup())
                return;
            if (MessageBox.Show(this, "Вы уверенны, что хотите удалить группу? Будет удалена только указанная группа. " +
                "Тренер и остальные группы будут не тронуты", "Удадение группы", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ApplicationContext DB = new();
                // удаляем группу тренера
                var GroupForDelete = DB.Groups.Where(x => x.Group == (CbGroups.SelectedItem as Label).Content.ToString() &&
                x.Trainer == (CbTrainers.SelectedItem as Label).Content.ToString()).ToArray();
                DB.Groups.RemoveRange(GroupForDelete);
                DB.SaveChanges();
                OwnerWindow.UpdateTrainers();
                this.Close();
            }
        }
    }
}
