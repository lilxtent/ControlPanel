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

namespace ControlPanel.Model
{
    
    class CameraModel
    {
        public FilterInfoCollection videoDevices { get; private set; }
        public VideoCaptureDevice videoSource { get; private set; }
        private ZXing.BarcodeReader reader { get; set; }

        private CamWindow window;
        private System.Windows.Controls.Image imageContainer;

        delegate void SetImageDelegate(Bitmap parameter);


        public CameraModel(CamWindow window, System.Windows.Controls.Image imageContainer) {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            reader = new ZXing.BarcodeReader();
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);
            this.window = window;
            this.imageContainer = imageContainer;
        }


        public void startVideo()
        {
            videoSource = new VideoCaptureDevice(videoDevices[window.lbCams.SelectedIndex].MonikerString);
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
                window.SetResult(result.Text);
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


        private void SetImageCam(Bitmap bitmap)
        {
            if (window.CheckAccess())
                window.imageCam.Source = ConvertBitmapToImage(bitmap);
            else
                window.Dispatcher.Invoke(new SetImageDelegate(SetImageCam), new object[] { bitmap });
        }
        
    }
}
