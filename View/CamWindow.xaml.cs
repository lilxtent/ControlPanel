using AForge.Video.DirectShow;
using ControlPanel.Model;
using ControlPanel.Services;
using ControlPanel.Sourses;
using System.ComponentModel;
using System.Windows;

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
            string cameraNameInConfig = ClientMethods.GetCameraNameConfig();

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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Camera.stopVideo();
        }
    }
}
