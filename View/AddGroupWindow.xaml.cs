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
    /// Логика взаимодействия для AddGroupWindow.xaml
    /// </summary>
    public partial class AddGroupWindow : Window
    {
        MainWindow OwnerWindow { get; set; }
        public AddGroupWindow(MainWindow owner)
        {
            InitializeComponent();
            // инициализируем список тренеров
            ApplicationContext DB = new();
            foreach (var trainer in DB.Trainers.ToList())
            {
                CbTrainers.Items.Add(new Label() { Content = trainer.ShortFullname });
            }
            OwnerWindow = owner;
        }
        private void ThisFieldCantBeEmpty(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть пустым: {fieldName}", "Ошибка");
        }
        private bool CheckAllTextBox()
        {
           
            if (TbGroup.Text is null || TbGroup.Text == "")
            {
                ThisFieldCantBeEmpty("Группа");
                return false;
            }
            if (CbTrainers.SelectedIndex == -1)
            {
                ThisFieldCantBeEmpty("Тренер");
                return false;
            }
            return true;
        }
        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckAllTextBox()) return;
            GroupModel Group = new((CbTrainers.SelectedItem as Label).Content.ToString(), TbGroup.Text);
            ApplicationContext DB = new();
            DB.Groups.Add(Group);
            DB.SaveChanges();
            this.Close();
        }
    }
}
