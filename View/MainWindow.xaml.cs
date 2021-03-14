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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ControlPanel.Services;
using ControlPanel.Model;
using ControlPanel.View;
using ControlPanel.Sourses;
using System.Windows.Threading;
using ControlPanel.ViewModel;
using ControlPanel.ViewModel.MainWindow;
using AForge.Video.DirectShow;
using AForge.Video;
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
            camWindow.Show();
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
                ButtonPaymentJournal.IsEnabled = true;//активируем кнопку журнала оплат
                ButtonVisitJournal.IsEnabled = true;//активируем кнопку журнала посещений

                var lbi = (ClientModelInfo)(lbClients.SelectedItem as ListBoxItem).Content;
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
                ButtonPaymentJournal.IsEnabled = false;//деактивируем кнопку просмотра журнала
                ButtonVisitJournal.IsEnabled = false;//деактивируем кнопку журнала посещений

                spPersonalArea.Children.Clear();
                spPayment.Children.Clear();
                spAdditionalInfo.Children.Clear();
            }
        }

        private void butAddClient_Click(object sender, RoutedEventArgs e)
        {
            AddUserWindow userWindow = new AddUserWindow();
            userWindow.Show();
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
            foreach (ClientModel Client in DB.ClientsModels.ToList())
                Box.Items.Add(new ListBoxShortClientData(Client));
        }

        public void UpdateClientsList(ListBox Box)
        {
            DB = new ApplicationContext();
            ShowAllClientsShortData(Box);
        }

        private void UpdateList_Click(object sender, RoutedEventArgs e)
        {
            UpdateClientsList(lbClients);
        }

        private void ButtonPaymentJournal_Click(object sender, RoutedEventArgs e)
        {
            PaymentJournalWindow Window = 
                new PaymentJournalWindow(
                    ((ClientModelInfo)(lbClients.SelectedItem as ListBoxItem).Content).clientModel);
            Window.ShowDialog();
        }

        private void ButtonVisitJournal_Click(object sender, RoutedEventArgs e)
        {
            VisitJournalWindow Window =
                new VisitJournalWindow(
                    ((ClientModelInfo)(lbClients.SelectedItem as ListBoxItem).Content).clientModel);
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
            EditClientProfile Editor = new EditClientProfile(((ClientModelInfo)(lbClients.SelectedItem as ListBoxItem).Content).clientModel);
            Editor.Show();
        }

    }
}
