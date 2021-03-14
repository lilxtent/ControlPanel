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
using System.ComponentModel;
using System.Configuration;
using System.Xml;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для CamWindow.xaml
    /// </summary>
    public partial class CamWindow : Window
    {
        delegate void SetStringDelegate(string parameter);
        private ApplicationContext DB { get; set; }
        private CameraModel Camera;

        public CamWindow(CameraModel cameraModel)
        {
            InitializeComponent();
            DB = new ApplicationContext();
            Camera = cameraModel;
            Camera.ImageContainer = imageCam;
            Camera.WindowCurr = this;
        }

        private void butONCam_Click(object sender, RoutedEventArgs e)
        {
            Camera.startVideo();
        }
        private int FindIndexCameraInConfig(FilterInfoCollection Devises)
        {
            string cameraNameInConfig = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Config.xml");

            foreach (XmlNode xNode in xDoc.ChildNodes)
                if (xNode.Name == "Camera")
                    cameraNameInConfig = xNode.InnerText;

            for (int i = 0; i < Devises.Count; i++)
                if (cameraNameInConfig == Devises[i].Name)
                    return i;
            return 0;
        }
        
        private void lbCams_Loaded(object sender, RoutedEventArgs e)
        {
            if (Camera.videoDevices.Count > 0)
            {
                foreach (FilterInfo device in Camera.videoDevices)
                {
                    lbCams.Items.Add(device.Name);
                }
                lbCams.SelectedIndex = FindIndexCameraInConfig(Camera.videoDevices);
            }
        }
        public string FindClient(string code)
        {
            string answer = "None";
            List<ClientModel> clients = DB.ClientsModels.ToList();
            string startStr = "https://pay-tomsk.ru/qr/01CA433100";
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
