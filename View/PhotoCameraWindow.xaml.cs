using AForge.Imaging.Filters;
using AForge.Video;
using AForge.Video.DirectShow;
using ControlPanel.Sourses;
using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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
        private bool isGoodImage { get; set; }


        delegate void SetImageDelegate(Bitmap parameter);
        delegate void SetEnableDelegate(bool parameter);



        public PhotoCameraWindow(Window currWindow)
        {
            InitializeComponent();
            VideoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            CurrWindow = currWindow;
            PhotoInterface = (IPhoto<Window>)CurrWindow;
            ImageConteiner = PhotoInterface.getImageConteiner();
            IdConteiner = PhotoInterface.getIdConteiner();
            ImageConteiner.Source = new BitmapImage(new Uri(ClientMethods.GetDefaultImagePathAbsolute()));
            isGoodImage = false; // проверка изображения с камеры на подходящий размер
            PhotoInterface.setChangedCameraStatus(false); // если мы выключали основную камеру пока фотались


        }
        private void butONCam_Click(object sender, RoutedEventArgs e)
        {
            // если для фото мы выбрали ту же камеру то приостанавливаем на время фото
            bool isCameraCheck = (CurrWindow.Owner as MainWindow).Camera.isCameraStart;
            if (!PhotoInterface.getChangedCameraStatus() && isCameraCheck && lbCams.SelectedItem.ToString() == ClientMethods.GetCameraNameConfig())
            {
                (CurrWindow.Owner as MainWindow).CheckBoxCameraOn.IsChecked = false;
                PhotoInterface.setChangedCameraStatus(true);
                MessageBox.Show("Вы выбрали текущую камеру для считывания кода!\n" +
                    "Её работа временно приостановленна до закрытия окна редактирования", "Предупреждение");
            }

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
            imageCam.Source = null; // обнуляем картинку
            isGoodImage = false;
            ButPhotoSave.IsEnabled = false;

            // очищаем предыдущий 
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
            // если ширина изображения будет больше 50 пикселей то мы разрешаем фотать
            if (!isGoodImage && Cadr.Width >= 50)
            {
                SetEnableSave(true);
                isGoodImage = true;
            }
            // и наоборот
            else if (isGoodImage && Cadr.Width < 50)
            {
                SetEnableSave(false);
                isGoodImage = false;
            }

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
        private void SetEnableSave(bool flag)
        {
            if (this.CheckAccess())
                ButPhotoSave.IsEnabled = flag;
            else
                this.Dispatcher.Invoke(new SetEnableDelegate(SetEnableSave), new object[] { flag });
        }
        private void ButPhotoSave_Click(object sender, RoutedEventArgs e)
        {
            string pathToPhotosDir = ConfigurationManager.AppSettings["PhotosDirPath"].ToString();

            string path = "pic" + IdConteiner.Text + ".jpg";
            Cadr.Save(pathToPhotosDir + path, ImageFormat.Jpeg);
            // выключаем камеру
            VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
            VideoSource.SignalToStop();
            // меняем изображение
            ImageConteiner.Source = new BitmapImage(new Uri(pathToPhotosDir + path, UriKind.Absolute));
            ImageConteiner.DataContext = path;
            this.Close(); // закрываем окно
        }

        private void ButtonCancel_Click(object sender, RoutedEventArgs e)
        {
            // выключаем камеру
            VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
            VideoSource.SignalToStop();
            this.Close(); // закрываем окно
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (VideoSource is not null)
            {
                VideoSource.NewFrame -= new NewFrameEventHandler(videoNewFrame);
                VideoSource.SignalToStop();
            }
        }
    }
}
