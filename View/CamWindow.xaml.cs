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
using ControlPanel.Services;
using ControlPanel.Model;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для CamWindow.xaml
    /// </summary>
    public partial class CamWindow : Window
    {
        private CameraModel camera { get; set; }
        delegate void SetStringDelegate(string parameter);
        private ApplicationContext DB { get; set; }

        public CamWindow()
        {
            InitializeComponent();
            DB = new ApplicationContext();
            camera = new CameraModel(this, imageCam);
        }

        private void butONCam_Click(object sender, RoutedEventArgs e)
        {
            camera.startVideo();
            SetResult("");
        }
        public void SetResult(string result)
        {
            if (CheckAccess())
                tbCode.Text = FindClient(result);
            else
                Dispatcher.Invoke(new SetStringDelegate(SetResult), new object[] { result });
        }
        private void lbCams_Loaded(object sender, RoutedEventArgs e)
        {
            if (camera.videoDevices.Count > 0)
            {
                foreach (FilterInfo device in camera.videoDevices)
                {
                    lbCams.Items.Add(device.Name);
                }
                lbCams.SelectedIndex = 0;
            }
        }
        public string FindClient(string code)
        {
            string answer = "None";
            List<ClientModel> clients = DB.ClientsModels.ToList();
            string startStr = "E700000216086";
            for (int i = 0; i < clients.Count; i++)
            {
                if (startStr + clients[i].ID.ToString() == code)
                {
                    answer = $"{clients[i].Surname} {clients[i].Name} {clients[i].PhoneNumber}";
                    break;
                }
            }
            return answer;
        }
    }
}
