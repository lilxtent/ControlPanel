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
using ControlPanel.Services;
using Microsoft.Win32;
using ControlPanel.Sourses;
using System.Drawing;
using System.IO;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для AddUserWindow.xaml
    /// </summary>
    public partial class AddUserWindow : Window, IPhoto<Window>
    {
        private ClientModel enterdClientData { get; set; }
        public bool isChangedCameraStatus { get; set; }
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

        private AddUserWindow()
        {
            InitializeComponent();
        }

        public AddUserWindow(Window owner)
        {
            InitializeComponent();
            Owner = owner;
            (owner as MainWindow).ClearSelectedClient();
            ProfilePicture.DataContext = ClientMethods.GetDefaultImagePathRelative();
        }
        // интерфейс IPhoto
        System.Windows.Controls.Image IPhoto<Window>.getImageConteiner()
        {
            return ProfilePicture;
        }
        TextBox IPhoto<Window>.getIdConteiner()
        {
            return ID;
        }
        bool IPhoto<Window>.getChangedCameraStatus()
        {
            return isChangedCameraStatus;
        }
        void IPhoto<Window>.setChangedCameraStatus(bool flag)
        {
            isChangedCameraStatus = flag;
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

            if (ID.Text.Trim(' ').Length != 6)
            {
                MessageBox.Show(this, "Поле 'ID' должно содержать 6 цифр", "Ошибка");
                return;
            }

            if (ClientSection.Text is null || ClientSection.Text.Trim(' ') == "")
            {
                ThisFieldCantBeEmpty("Секция");
                return;
            }

            EnterdClientData = new ClientModel(Convert.ToInt32(ID.Text.Trim(' ')),
                                               ClientSurname.Text.Trim(' '), ClientName.Text.Trim(' '),
                                               ClientPatronymic.Text.Trim(' '),
                                               (DateTime)ClientBirthDate.SelectedDate,
                                               ClientPhoneNumber.Text.Trim(' '),
                                               default(DateTime),
                                               ClientSection.Text.Trim(' '),
                                               ProfilePicture.DataContext.ToString(),
                                               ClientParentType.Text.Trim(' '),
                                               ClientParentSurname.Text.Trim(' ') + " " + ClientParentName.Text.Trim(' ') + " " + ClientParentPatronymic.Text.Trim(' '),
                                               ClientParentPhoneNumber.Text.Trim(' '),
                                               default(DateTime)
                                               );
            ApplicationContext DB = new ApplicationContext();
            if (DB.ClientsModels.Find(new object[] { EnterdClientData.ID }) is not null)
            {
                MessageBox.Show(this, "Клиент с таким ID уже существует", "Ошибка");
                return;
            }
            DB.ClientsModels.Add(EnterdClientData);
            DB.SaveChanges();
            Close();
            //Обновляем информацию о клиентах в главном окне
            (Owner as MainWindow).UpdateClientsList((Owner as MainWindow).lbClients);
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
                BitmapImage TempBtm = new BitmapImage(new Uri(dialog.FileName));
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(TempBtm));
                string photoPath = @"\Photos\" + ClientMethods.GetOnlyFileName(dialog.FileName.ToString());
                using (FileStream filestream = new(ClientMethods.ConvertRelativeToAbsolutePath(photoPath), FileMode.Create))
                {
                    encoder.Save(filestream);
                }
                ProfilePicture.Source = new BitmapImage(new Uri(ClientMethods.ConvertRelativeToAbsolutePath(photoPath)));
                ProfilePicture.DataContext = photoPath;
                MessageBox.Show(ClientMethods.GetOnlyFileName(dialog.FileName.ToString()));

            }
        }

        private void ButtonPhoto_Click(object sender, RoutedEventArgs e)
        {
            PhotoCameraWindow PhotoCamera = new(this);
            PhotoCamera.ShowDialog();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (isChangedCameraStatus)
                (Owner as MainWindow).CheckBoxCameraOn.IsChecked = true;
        }
    }
}
