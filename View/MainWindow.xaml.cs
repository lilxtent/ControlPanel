using AForge.Video;
using AForge.Video.DirectShow;
using ControlPanel.Model;
using ControlPanel.Services;
using ControlPanel.Sourses;
using ControlPanel.View;
using ControlPanel.ViewModel;
using ControlPanel.ViewModel.MainWindow;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isEnableAreaExtendSubscripion;
        private ApplicationContext DB { get; set; }
        public CameraModel Camera { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            DB = new ApplicationContext();

            if (!File.Exists(DB.PathToDataBase))
            {
                MessageBox.Show("Указан неверный путь к базе данных. Укажите новый.");
                ShowDialogPathToDB();
            }

            Camera = new CameraModel(DB);
            isEnableAreaExtendSubscripion = false;
            CheckBoxCameraOn.Background = ClientMethods.GetRedColorBrush();
            TodayVisits.ItemsSource = new TodayVisitsList();
            Camera.NewClientArrived += x =>
            {
                (TodayVisits.ItemsSource as TodayVisitsList).Add(new ShortVisitViewModel(x.FIO, x.DateLastVisit));
                TodayVisits.Items.Refresh();
            };
            UpdateTrainers(); // инициализируем список тренеров
        }
        // инициализируем список тренеров
        public void UpdateTrainers()
        {
            ComboBoxTrainers.Items.Clear(); // очищаем
            ApplicationContext DB = new();
            // первый элемент это любой тренер
            ComboBoxTrainers.Items.Add(new Label() { Content = "Любой тренер" });
            foreach (TrainerModel trainer in DB.Trainers.ToList())
                ComboBoxTrainers.Items.Add(new Label() { Content = trainer.ShortFullname });
            ComboBoxTrainers.SelectedIndex = 0; // выбираем "Любой тренер" по умолчанию
        }

        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAllClientsShortData(lbClients);
        }

        private void miSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            CheckBoxCameraOn.IsChecked = false; // при переключении в настройки камеры выключаем камеру
            var camWindow = new CamWindow(Camera);
            camWindow.ShowDialog();
        }
        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            // Выгружаем название камеры из конфигураций
            string cameraNameInConfig = ClientMethods.GetCameraNameConfig();
            // ищем камеру с таким же названием среди Devices
            int indexCamera = -1;
            for (int i = 0; i < Camera.videoDevices.Count; i++)
                if (cameraNameInConfig == Camera.videoDevices[i].Name)
                {
                    indexCamera = i;
                    break;
                }
            // после нахождения инициализируем съемку
            if (indexCamera != -1)
            {
                Camera.videoSource = new VideoCaptureDevice(Camera.videoDevices[indexCamera].MonikerString);
                Camera.videoSource.NewFrame += new NewFrameEventHandler(Camera.videoNewFrame);
                Camera.videoSource.Start();
                Camera.isCameraStart = true;
            }
            // если не нашли выводим предупреждение
            else
            {
                MessageBox.Show("Указанной камеры по умолчанию не существует\nЗайдите в раздел Настройки->Камера",
                    "Предупреждение");
            }

        }
        private void lbClients_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbClients.SelectedIndex != -1)
            {
                isEnableAreaExtendSubscripion = false; // деактивируем продление абонемента

                ButtonPaymentJournal.IsEnabled = true; // активируем кнопку журнала оплат
                ButtonVisitJournal.IsEnabled = true; // активируем кнопку журнала посещений
                butExtendSubscription.IsEnabled = true; // активируем кнопку редактирования
                butEdit.IsEnabled = true; // активируем кнопку продления абонемента
                ButtonVisit.IsEnabled = true; // активируем кнопку отметить посещение

                var lbi = (ClientModel)((lbClients.SelectedItem as Grid).DataContext);
                // список объектов для общей информации о спортсмене(клиенте)
                PersonalUnit[] personalObj = {
                new PersonalAvatar(lbi),
                new PersonalFIO(lbi),
                new PersonalSection(lbi),
                new PersonalTrainer(lbi),
                new PersonalGroup(lbi),
                new PersonalPhone(lbi),
                new PersonalBirthDate(lbi),
                new PersonalLastPay(lbi),
                new PersonalLastVisit(lbi),
                };
                spPersonalArea.Children.Clear();
                spPayment.Children.Clear();
                spAdditionalInfo.Children.Clear();

                foreach (var el in personalObj)
                    spPersonalArea.Children.Add(el.getGrid());

                // список объектов для дополнительной информации
                PersonalUnit[] AdditionalInfoObj = {
                new AdditionalInfoParent(lbi),
                new AdditionalInfo(lbi)
                };
                foreach (var el in AdditionalInfoObj)
                    spAdditionalInfo.Children.Add(el.getGrid());
            }
            else
            {
                ButtonPaymentJournal.IsEnabled = false; // деактивируем кнопку просмотра журнала
                ButtonVisitJournal.IsEnabled = false; // деактивируем кнопку журнала посещений
                butExtendSubscription.IsEnabled = false; // деактивируем кнопку редактирования
                butEdit.IsEnabled = false; // деактивируем кнопку продления абонемента
                ButtonVisit.IsEnabled = false; // деактивируем кнопку отметить посещение


                spPersonalArea.Children.Clear();
                spPayment.Children.Clear();
                spAdditionalInfo.Children.Clear();
            }
        }

        private void butAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow userWindow = new AddUserWindow(this);
            userWindow.ShowDialog();
        }

        private void TextBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TextBoxSearch.Text is null || TextBoxSearch.Text.Trim(' ') is "")
            {
                ShowAllClientsShortData(lbClients);
            }
            else
            {
                lbClients.Items.Clear();
                foreach (ClientModel Client in DB.ClientsModels.ToList())
                {
                    if (Client.FIO.ToLower().Contains(TextBoxSearch.Text.Trim(' ').ToLower()))
                    {
                        lbClients.Items.Add(new ListBoxShortClientData(Client));
                    }
                }
            }
        }

        /// <summary>
        /// Очищает переданный ListBox и выводит в нем краткую информацию о всех клиентах
        /// </summary>
        public void ShowAllClientsShortData(ListBox Box)
        {
            Box.Items.Clear();
            ClientModel[] Clients = DB.ClientsModels.ToArray();
            // если мы выбрали тренера 
            if (ComboBoxTrainers.SelectedIndex > 0)
                Clients = DB.ClientsModels.ToList().Where(x => x.Trainer == (ComboBoxTrainers.SelectedItem as Label).Content.ToString()).ToArray();
            // если мы выбрали группу
            if (ComboBoxGroups.SelectedIndex > 0)
                Clients = Clients.ToList().Where(x => x.Group == (ComboBoxGroups.SelectedItem as Label).Content.ToString()).ToArray();
            

            SortCLientModel(Clients);
            foreach (ClientModel Client in Clients)
            {
                Grid CurrGrid = new ClientArea(Client).getGrid();
                CurrGrid.Width = 1;
                CurrGrid.ClearValue(GridViewColumn.WidthProperty);
                Box.Items.Add(CurrGrid);
            }

        }

        public void UpdateClientsList(ListBox Box)
        {
            ShowAllClientsShortData(Box);
        }

        private void ButtonPaymentJournal_Click(object sender, RoutedEventArgs e)
        {
            PaymentJournalWindow Window = 
                new PaymentJournalWindow((ClientModel)(lbClients.SelectedItem as Grid).DataContext);
            Window.ShowDialog();
        }

        private void ButtonVisitJournal_Click(object sender, RoutedEventArgs e)
        {
            VisitJournalWindow Window =
                new VisitJournalWindow((ClientModel)(lbClients.SelectedItem as Grid).DataContext);

            Window.ShowDialog();
        }

        private void butExtendSubscription_Click(object sender, RoutedEventArgs e)
        {
            // если клиент не выбран сообщить
            if (lbClients.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите клиента из списка!");
                return;
            }
            // если мы уже активировали поле для продления абонемента мы его закроем и наоборот
            if (isEnableAreaExtendSubscripion)
            {
                spPayment.Children.Clear();
                isEnableAreaExtendSubscripion = false;
            }
            else
            {
                ExtendSubscription AreaExtendSubscripion = new ExtendSubscription(this);
                AreaExtendSubscripion.Show();
                isEnableAreaExtendSubscripion = true;
            }
        }

        private void butEdit_Click(object sender, RoutedEventArgs e)
        {
            // если клиент не выбран сообщить
            if (lbClients.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите клиента из списка!");
                return;
            }
            // инициализируем окно редактирования
            EditClientProfile Editor = new EditClientProfile(this,(ClientModel)(lbClients.SelectedItem as Grid).DataContext);
            Editor.ShowDialog();
        }

        private void SortCLientModel(ClientModel[] Clients)
        {
            if (SortOptionsList.SelectedIndex == 0)
                Array.Sort(Clients, ClientModel.SortOderAlphabetIncrease());
            else if (SortOptionsList.SelectedIndex == 1)
                Array.Sort(Clients, ClientModel.SortOderAlphabetDecrease());
            else if (SortOptionsList.SelectedIndex == 2)
                Array.Sort(Clients, ClientModel.SortOderDateLastPaymentNewer());
            else if (SortOptionsList.SelectedIndex == 3)
                Array.Sort(Clients, ClientModel.SortOderDateLastPaymentOlder());
        }

        private void SortOptionsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsInitialized) ShowAllClientsShortData(lbClients);
        }
        public void ClearSelectedClient()
        {
            lbClients.SelectedIndex = -1;
        }



        private void CheckBoxCameraOn_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBoxCameraOn.Content = "Камера выключена";
            CheckBoxCameraOn.Background = ClientMethods.GetRedColorBrush();
            Camera.stopVideo();
            
        }

        private void CheckBoxCameraOn_Checked(object sender, RoutedEventArgs e)
        {

            // Выгружаем название камеры из конфигураций
            string cameraNameInConfig = ClientMethods.GetCameraNameConfig();
            // ищем камеру с таким же названием среди Devices
            int indexCamera = -1;
            for (int i = 0; i < Camera.videoDevices.Count; i++)
                if (cameraNameInConfig == Camera.videoDevices[i].Name)
                {
                    indexCamera = i;
                    break;
                }
            // после нахождения инициализируем съемку
            if (indexCamera != -1)
            {
                Camera.videoSource = new VideoCaptureDevice(Camera.videoDevices[indexCamera].MonikerString);
                Camera.videoSource.NewFrame += new NewFrameEventHandler(Camera.videoNewFrame);
                Camera.videoSource.Start();
                Camera.isCameraStart = true;
                CheckBoxCameraOn.Content = "Камера включена";
                CheckBoxCameraOn.Background = ClientMethods.GetGreenColorBrush();

            }
            // если не нашли выводим предупреждение
            else
            {
                MessageBox.Show("Указанной камеры по умолчанию не существует\nЗайдите в раздел Настройки->Камера",
                    "Предупреждение");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (Camera.videoSource is not null)
            {
                Camera.videoSource.NewFrame -= new NewFrameEventHandler(Camera.videoNewFrame);
                Camera.videoSource.SignalToStop();
            }
        }

        private void ButtonAddTrainer_Click(object sender, RoutedEventArgs e)
        {
            AddTrainerWindow TrainerWindow = new(this);
            TrainerWindow.ShowDialog();
        }

        private void ComboBoxTrainers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAllClientsShortData(lbClients);
            // группы
            ComboBoxGroups.Items.Clear();
            ComboBoxGroups.Items.Add(new Label() { Content = "все группы" });
            foreach (var group in DB.Groups.ToList().Where(x => x?.Trainer == (ComboBoxTrainers.SelectedItem as Label)?.Content.ToString()))
            {
                ComboBoxGroups.Items.Add(new Label() { Content = group.Group });
            }
            ComboBoxGroups.SelectedIndex = 0; // изначально выбираем первую
        }

        private void ButtonVisit_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show(this, "Отметить посещение?", "Подтверждение", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ClientModel Client = (lbClients.SelectedItem as Grid).DataContext as ClientModel;
                PopUpWindow popupWindow = new(Client, Camera);
                popupWindow.Show();

                (TodayVisits.ItemsSource as TodayVisitsList).Add(new ShortVisitViewModel(Client.FIO, Client.DateLastVisit));
                TodayVisits.Items.Refresh();
            }
        }
        private void ComboBoxGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowAllClientsShortData(lbClients);

        }

        private void ButtonAddGroup_Click(object sender, RoutedEventArgs e)
        {
            AddGroupWindow GroupWindow = new(this);
            GroupWindow.ShowDialog();
        }

        private void ButtonDeleteTrainer_Click(object sender, RoutedEventArgs e)
        {
            DeleteTrainerWindow DeleteTrainer = new(this);
            DeleteTrainer.ShowDialog();
        }

        private void ChoosePathToDB(object sender, RoutedEventArgs e)
        {
            ShowDialogPathToDB();
        }

        private void ShowDialogPathToDB()
        {
            OpenFileDialog dlg = new();
            dlg.DefaultExt = ".db";
            dlg.Filter = "Файл базы данных (.db)|*.db";

            if (dlg.ShowDialog() is true)
            {
                DB.PathToDataBase = dlg.FileName;
                DB = new ApplicationContext();
                ShowAllClientsShortData(lbClients);
                TodayVisits.ItemsSource = new TodayVisitsList();
            }
        }
    }
}
