using System;
using System.Collections.Generic;
using System.Text;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing;
using System.IO;
using ControlPanel;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using ControlPanel.View;
using System.Windows;

namespace ControlPanel.Model
{

    public class CameraModel
    {
        public FilterInfoCollection videoDevices { get; private set; }
        public VideoCaptureDevice videoSource { get; private set; }
        private ZXing.BarcodeReader reader { get; set; }
        private CamWindow windowCurr;

        private System.Windows.Controls.Image imageContainer;

        delegate void SetImageDelegate(Bitmap parameter);

        public CamWindow WindowCurr
        {
            get { return windowCurr; }
            set { windowCurr = value; }
        }
        public System.Windows.Controls.Image ImageContainer
        {
            get { return imageContainer; }
            set { imageContainer = value; }
        }


        public CameraModel() {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            reader = new ZXing.BarcodeReader();
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
        }


        public void startVideo()
        {
            videoSource = new VideoCaptureDevice(videoDevices[windowCurr.lbCams.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoNewFrame);
            videoSource.Start();
        }
        private void videoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
            SetImageCam(bitmap);

            ZXing.Result result = reader.Decode((Bitmap)eventArgs.Frame.Clone());

            if (result != null)
            {
                windowCurr.SetResult(result.Text);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var popupWindow = new PopUpWindow(result.Text) { Name = "popupWindow"};
                    popupWindow.Show();
                });
                
            }
        }
        public BitmapImage ConvertBitmapToImage(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();   
            return image;
        }
        public static BitmapImage ToBitmapImage(Bitmap bitmap)
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
            if (windowCurr.CheckAccess())
                windowCurr.imageCam.Source = (System.Windows.Media.ImageSource)ToBitmapImage(bitmap);
            else
                windowCurr.Dispatcher.Invoke(new SetImageDelegate(SetImageCam), new object[] { bitmap });
        }
        
    }
}
