using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ControlPanel.Model;
using ControlPanel.Services;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для PopUpWindow.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        private DispatcherTimer timer { get; set; }
        private ClientModel Client { get; set; }
        private CameraModel Camera { get; set; }
        public PopUpWindow(ClientModel Client, CameraModel Camera)
        {
                
            
            this.Client = Client;
            this.Camera = Camera;
            InitializeComponent();
            InitializeClientInfo();
            
            Camera.isPopUpWindowActive = true;
            this.Top = 10;
            this.Left = 20;
            this.WindowStyle = WindowStyle.None;
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
        }
        private void timerTick(object sender, EventArgs e)
        {
            timer.Stop();
            Camera.isPopUpWindowActive = false;
            this.Hide();
        }
        private void InitializeClientInfo()
        {
            labelFIO.HorizontalContentAlignment = HorizontalAlignment.Center;
            if (Client is not null)
            {

                labelFIO.Content = $"{Client.Surname} {Client.Name} {Client.Patronymic}";
                imagePhoto.Source = new BitmapImage(new Uri(Client.PhotoPath, UriKind.Absolute));
                this.Background = new SolidColorBrush(Color.FromArgb(255, 0, 244, 137));
            }
            else
            {
                imagePhoto.Source = new BitmapImage(new Uri(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Sourses\Images\default-user-image.png",
                    UriKind.Absolute));
                this.Background = new SolidColorBrush(Color.FromArgb(255, 254, 244, 137));
            }
            
        }
    }
}
