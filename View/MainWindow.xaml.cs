using AForge.Video;
using AForge.Video.DirectShow;
using ControlPanel.Model;
using ControlPanel.Services;
using ControlPanel.Sourses;
using ControlPanel.View;
using ControlPanel.ViewModel;
using ControlPanel.ViewModel.MainWindow;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace ControlPanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isEnableAreaExtendSubscripion;

        private ApplicationContext DB { get; set; }
        private CameraModel Camera { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DB = new ApplicationContext();
            Camera = new CameraModel(DB);
            isEnableAreaExtendSubscripion = false;
            
        }

        private void lbClients_Loaded(object sender, RoutedEventArgs e)
        {
            ShowAllClientsShortData(lbClients);
        }

        private void miSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            var camWindow = new CamWindow(Camera);
            camWindow.ShowDialog();
        }
        private void butSetupCamera_Click(object sender, RoutedEventArgs e)
        {
            // Выгружаем название камеры из конфигураций
            string cameraNameInConfig = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Config.xml");
            foreach (XmlNode xNode in xDoc.ChildNodes)
                if (xNode.Name == "Camera")
                    cameraNameInConfig = xNode.InnerText;
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
                ButtonPaymentJournal.IsEnabled = true; // активируем кнопку журнала оплат
                ButtonVisitJournal.IsEnabled = true; // активируем кнопку журнала посещений
                butExtendSubscription.IsEnabled = true; // активируем кнопку редактирования
                butEdit.IsEnabled = true; // активируем кнопку продления абонемента
                MessageBox.Show(lbClients.SelectedItem.ToString());
                var lbi = (ClientModel)((lbClients.SelectedItem as Grid).DataContext);
                // список объектов для общей информации о спортсмене(клиенте)
                PersonalUnit[] personalObj = {
                new PersonalAvatar(lbi),
                new PersonalFIO(lbi),
                new PersonalSection(lbi),
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
                new AdditionalInfoParent(lbi)
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
        private void ShowAllClientsShortData(ListBox Box)
        {
            Box.Items.Clear();

            ClientModel[] Clients = DB.ClientsModels.ToArray();
            SortCLientModel(Clients);
            foreach (ClientModel Client in Clients)
                Box.Items.Add(new ClientArea(Client).getGrid());
        }

        public void UpdateClientsList(ListBox Box)
        {
            DB = new ApplicationContext();
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
    }
}
