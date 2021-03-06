using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ControlPanel.Model;
using Microsoft.Win32;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window
    {
        private ClientModel enterdClientData { get; set; }
        public ClientModel EnterdClientData
        {
            get
            {
                if (enterdClientData is null)
                    throw new NullReferenceException();
                return enterdClientData;
            }
            private set { enterdClientData = value; }
        }

        public AddUserWindow()
        {
            InitializeComponent();
        }

        private void ButtonAdd_Click(object sender, RoutedEventArgs e)
        {
            if (ClientSurname.Text is null || ClientSurname.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Фамилия");
                return;
            }
            if (ClientName.Text is null || ClientName.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Имя");
                return;
            }
            if (ClientPatronymic.Text is null || ClientPatronymic.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Отчество");
                return;
            }
            if (ClientBirthDate.SelectedDate is null)
            {
                ThisFieldCantBeEmpty("Дата рождения");
                return;
            }
            if (ClientPhoneNumber.Text is null || ClientPhoneNumber.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Номер телефона");
                return;
            }

            if (ClientPhoneNumber.Text.Trim(' ').Length != 11)
            {
                MessageBox.Show(this, "Поле 'Номер телефона' должно содержать 11 цифр", "Ошибка");
                return;
            }

            if(ID.Text is null || ID.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("ID");
                return;
            }

            if (ID.Text.Trim(' ').Length != 10)
            {
                MessageBox.Show(this, "Поле 'ID' должно содержать 10 цифр", "Ошибка");
                return;
            }

            EnterdClientData = new ClientModel(Convert.ToInt32(ID.Text.Trim(' ')),
                                               ClientSurname.Text.Trim(' '), ClientName.Text.Trim(' '),
                                               ClientPatronymic.Text.Trim(' '),
                                               (DateTime)ClientBirthDate.SelectedDate,
                                               ClientPhoneNumber.Text.Trim(' '),
                                               default(DateTime),
                                               ProfilePicture.Source.ToString());
            Close();
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ThisFieldCantBeEmpty(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть пустым: {fieldName}", "Ошибка");
        }

        private void ChosePhoto(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "jpg files (*.jpg)|*.jpg|png files (*.png)|*.png";
            dialog.Title = "Выберите фотографию клиента";
            if (dialog.ShowDialog() ?? false)
            {
                ProfilePicture.Source = new BitmapImage(new Uri(dialog.FileName));
            }
        }
    }
}
