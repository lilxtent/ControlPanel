using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Imaging;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
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
using System.Windows.Shapes;
using System.Drawing.Imaging;
using ControlPanel.Sourses;
using AForge.Imaging.Filters;

namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для PhotoCameraWindow.xaml
    /// </summary>
    public partial class PhotoCameraWindow : Window
    {
        public FilterInfoCollection VideoDevices { get; set; }
        public VideoCaptureDevice VideoSource { get; set; }
        public Bitmap Cadr { get; set; }
        private Window CurrWindow { get; set; }
        private IPhoto<Window> PhotoInterface { get; set; }
        private System.Windows.Controls.Image ImageConteiner { get; set; }
        private TextBox IdConteiner { get; set; }

        delegate void SetImageDelegate(Bitmap parameter);


        public PhotoCameraWindow(Window currWindow)
        {
            InitializeComponent();
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            CurrWindow = currWindow;
            PhotoInterface = (IPhoto<Window>)CurrWindow;
            ImageConteiner = PhotoInterface.getImageConteiner();
            IdConteiner = PhotoInterface.getIdConteiner();
            ImageConteiner.Source = new BitmapImage(new Uri("pack://application:,,,/View/default-user-image.png"));


        }
        private void butONCam_Click(object sender, RoutedEventArgs e)
        {
            startVideo();
        }
        private void lbCams_Loaded(object sender, RoutedEventArgs e)
        {
            if (VideoDevices.Count > 0)
            {
                foreach (FilterInfo device in VideoDevices)
                {
                    lbCams.Items.Add(device.Name);
                }
                lbCams.SelectedIndex = 0;
            }
        }
        public void startVideo()
        {
            if (VideoSource is not null)
            {
                VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
                VideoSource.SignalToStop();
            }
            VideoSource = new VideoCaptureDevice(VideoDevices[lbCams.SelectedIndex].MonikerString);
            VideoSource.NewFrame += new NewFrameEventHandler(videoNewFrame);
            VideoSource.Start();
        }
        public void videoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            
            Cadr = (Bitmap)eventArgs.Frame.Clone();
            // соотношение 3/4 * w/h 
            double mult = Cadr.Width / Cadr.Height * 0.75;
            double multForSplit = (1 - mult) / 2;
            System.Drawing.Rectangle Rect = new System.Drawing.Rectangle((int)(Cadr.Width * multForSplit), 0,
                (int)(Cadr.Width * mult), Cadr.Height);
            Cadr = new Crop(Rect).Apply(Cadr);
            SetImageCam(Cadr);
        }
        public static BitmapImage ConvertBitmapToImage(Bitmap bitmap)
        {
            MemoryStream ms = new MemoryStream();
            bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;

        }
        private void SetImageCam(Bitmap bitmap)
        {
            if (this.CheckAccess())
                imageCam.Source = (System.Windows.Media.ImageSource)ConvertBitmapToImage(bitmap);
            else
                this.Dispatcher.Invoke(new SetImageDelegate(SetImageCam), new object[] { bitmap });
        }
        private void ButPhotoSave_Click(object sender, RoutedEventArgs e)
        {
            
            string path = Environment.CurrentDirectory + @"\..\..\..\Photos\pic"+ IdConteiner.Text + ".png";
            Cadr.Save(path, ImageFormat.Png);
            // выключаем камеру
            VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
            VideoSource.SignalToStop();
            // меняем изображение
            ImageConteiner.Source = new BitmapImage(new Uri(path, UriKind.Absolute)); 
            this.Close(); // закрываем окно
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // выключаем камеру
            VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
            VideoSource.SignalToStop();
            this.Close(); // закрываем окно
        }
    }
}
