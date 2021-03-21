using ControlPanel.Model;
using ControlPanel.Services;
using Microsoft.Win32;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using ControlPanel.Sourses;
using System.Windows.Controls;
using System.IO;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для EditClientProfile.xaml
    /// </summary>
    public partial class EditClientProfile : Window, IPhoto<Window>
    {
        private ClientModel clientData { get; set; }
        public bool isChangedCameraStatus { get; set; }
        public ClientModel ClientData
        {
            get
            {
                if (clientData is null)
                    throw new NullReferenceException();
                return clientData;
            }
            private set { clientData = value; }
        }

        private EditClientProfile()
        {
            InitializeComponent();
        }

        private EditClientProfile(ClientModel Client)
        {
            ClientData = Client;

            InitializeComponent();
            ClientSurname.Text = Client.Surname;
            ClientName.Text = Client.Name;
            ClientPatronymic.Text = Client.Patronymic;
            ClientBirthDate.SelectedDate = Client.BirthDate;
            ClientPhoneNumber.Text = Client.PhoneNumber;
            ID.Text = Convert.ToString(Client.ID);
            ClientSection.Text = Client.Section;

            ClientParentType.Text = Client.ParentType;
            if (Client.ParentFIO is not null)
            {
                string[] parentFIO = Client.ParentFIO.Split(' ');
                if (parentFIO.Length > 0) ClientParentSurname.Text = parentFIO[0];
                if (parentFIO.Length > 1) ClientParentName.Text = parentFIO[1];
                if (parentFIO.Length > 2) ClientParentPatronymic.Text = parentFIO[2];
            }
            ClientParentPhoneNumber.Text = Client.ParentPhoneNumber;
            Uri UriPath = new Uri(ClientMethods.ConvertRelativeToAbsolutePath(@"\Photos\default-user-image.png"));
            if (File.Exists(ClientMethods.ConvertRelativeToAbsolutePath(Client.PhotoPath)))
                UriPath = new Uri(ClientMethods.ConvertRelativeToAbsolutePath(Client.PhotoPath), UriKind.Absolute);
            ProfilePicture.Source = new BitmapImage(UriPath);
            ProfilePicture.DataContext = Client.PhotoPath;
        }
        public EditClientProfile(Window owner, ClientModel Client):this(Client)
        {
            Owner = owner;
            (owner as MainWindow).ClearSelectedClient();
            ProfilePicture.DataContext = Client.PhotoPath;
        }
        Image IPhoto<Window>.getImageConteiner()
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

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            ApplicationContext DB = new ApplicationContext();

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

            ClientModel newClientData = new ClientModel(Convert.ToInt32(ID.Text.Trim(' ')),
                                               ClientSurname.Text.Trim(' '), ClientName.Text.Trim(' '),
                                               ClientPatronymic.Text.Trim(' '),
                                               (DateTime)ClientBirthDate.SelectedDate,
                                               ClientPhoneNumber.Text.Trim(' '),
                                               ClientData.DateLastPayment,
                                               ClientSection.Text.Trim(' '),
                                               ProfilePicture.DataContext.ToString(), // мы берем относительный путь
                                               ClientParentType.Text.Trim(' '),
                                               ClientParentSurname.Text.Trim(' ') + " " + ClientParentName.Text.Trim(' ') + " " + ClientParentPatronymic.Text.Trim(' '),
                                               ClientParentPhoneNumber.Text.Trim(' '),
                                               ClientData.DateLastVisit
                                               );


            if (newClientData.ID != ClientData.ID)
            {
                ClientData.UpdateIDInDB(DB, newClientData.ID);
                DB.ClientsModels.Remove(ClientData);
                DB.ClientsModels.Add(newClientData);
            }
            else
            {
                DB.ClientsModels.Update(newClientData);
            }
            DB.SaveChanges();
            ClientData = newClientData;
            MessageBox.Show("Изменения сохранены");
            Close();
            //Обновляем информацию о клиентах в главном окне
            (Owner as MainWindow).UpdateClientsList((Owner as MainWindow).lbClients);
        }

        private void DeleteClient(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Вы уверенны, что хотите удалить клиента?", "Удадение клиента", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ApplicationContext DB = new ApplicationContext();
                ClientData.RemoveFromDB(DB);
                DB.SaveChanges();
                Close();
                //Обновляем информацию о клиентах в главном окне
                (Owner as MainWindow).UpdateClientsList((Owner as MainWindow).lbClients);
            }
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ThisFieldCantBeEmpty(string fieldName)
        {
            MessageBox.Show(this, $"Это поле не может быть пустым: {fieldName}", "Ошибка");
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
