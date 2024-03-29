﻿using ControlPanel.Model;
using ControlPanel.Services;
using ControlPanel.Sourses;
using Microsoft.Win32;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
            InitTrainers(); // заполняем ComboBoxTrainer тренерами

        }
        // инициализируем список тренеров
        private void InitTrainers()
        {
            ApplicationContext DB = new();
            foreach (TrainerModel trainer in DB.Trainers?.ToList())
                ComboBoxTrainer.Items.Add(new Label() { Content = trainer.ShortFullname });
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

            if (ID.Text is null || ID.Text.Trim(' ') == "")
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

            if (ComboBoxTrainer.SelectedIndex == -1)
            {
                ThisFieldCantBeEmpty("Тренер");
                return;
            }
            if (ComboBoxGroup.SelectedIndex == -1)
            {
                ThisFieldCantBeEmpty("Группа");
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
                                               default(DateTime), (ComboBoxTrainer.SelectedItem as Label).Content.ToString(),
                                               (ComboBoxGroup.SelectedItem as Label).Content.ToString()
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
                string pathToPhotosDir = ConfigurationManager.AppSettings["PhotosDirPath"].ToString();
                BitmapImage TempBtm = new BitmapImage(new Uri(dialog.FileName));
                var encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(TempBtm));
                string photoPath = pathToPhotosDir + ClientMethods.GetOnlyFileName(dialog.FileName.ToString());
                using (FileStream filestream = new(photoPath, FileMode.Create))
                {
                    encoder.Save(filestream);
                }
                ProfilePicture.Source = new BitmapImage(new Uri(photoPath));
                ProfilePicture.DataContext = ClientMethods.GetOnlyFileName(photoPath);
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

        private void ComboBoxTrainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationContext DB = new ApplicationContext();
            foreach (GroupModel group in DB.Groups?.ToList().Where(x => x.Trainer == (ComboBoxTrainer.SelectedItem as Label).Content.ToString()))
                ComboBoxGroup.Items.Add(new Label() { Content = group.Group });
        }
    }
}
