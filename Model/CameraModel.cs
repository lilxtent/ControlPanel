﻿using System;
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
using System.Configuration;
using System.Windows;
using ControlPanel.Services;
using System.Linq;
using System.Xml;

namespace ControlPanel.Model
{

    public class CameraModel
    {
        public FilterInfoCollection videoDevices { get;  set; }
        public VideoCaptureDevice videoSource { get;  set; }
        private ZXing.BarcodeReader reader { get; set; }
        private CamWindow windowCurr;
        private System.Windows.Controls.Image imageContainer;
        private ApplicationContext DB { get; set; }
        public bool isPopUpWindowActive;
        public bool isCameraStart;


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


        public CameraModel(ApplicationContext DB)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            reader = new ZXing.BarcodeReader();
            reader.Options.PossibleFormats = new List<ZXing.BarcodeFormat>();
            reader.Options.PossibleFormats.Add(ZXing.BarcodeFormat.QR_CODE);

            this.DB = new();
            isPopUpWindowActive = false;
            isCameraStart = false;
        }
        private void SetCameraInConfig(string newCameraName)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Config.xml");
            foreach (XmlNode xNode in xDoc.ChildNodes)
            {
                if(xNode.Name == "Camera")
                {
                    xNode.InnerText = newCameraName;
                }
            }
            xDoc.Save(@"C:\Users\ksh19\Desktop\Shadow\ControlPanel\Config.xml");
        }

        public void startVideo()
        {
            // если выбранная камера отличается от камеры в конфигурации мы перезаписываем конфигурацию
            if (windowCurr.lbCams.SelectedItem.ToString() != ConfigurationManager.AppSettings["CameraName"].ToString())
            {
                SetCameraInConfig(windowCurr.lbCams.SelectedItem.ToString());

            }
            videoSource = new VideoCaptureDevice(videoDevices[windowCurr.lbCams.SelectedIndex].MonikerString);
            videoSource.NewFrame += new NewFrameEventHandler(videoNewFrame);
            videoSource.Start();
            isCameraStart = true;
        }
        public void videoNewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (!isPopUpWindowActive)
            {
                Bitmap bitmap = (Bitmap)eventArgs.Frame.Clone();
                
                SetImageCam(bitmap);

                ZXing.Result result = reader.Decode((Bitmap)eventArgs.Frame.Clone());

                if (result != null)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        var popupWindow = new PopUpWindow(FindClient(result.Text), this) { Name = "popupWindow" };
                        popupWindow.Show();
                    });

                }
            }
        }
        private ClientModel FindClient(string id)
        {
            int idClient;
            if (int.TryParse(id, out idClient))
            {
                idClient = int.Parse(id);
            }
            else
            {
                return null;
            }
            List<ClientModel> Clients = DB.ClientsModels.ToList();
            foreach (ClientModel client in Clients)
            {

                if (client.ID == idClient)
                {
                    return client;
                }
            }
            return null;
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
            if (windowCurr is null) return; // если окно не инициализированно

            if (windowCurr.CheckAccess())
                windowCurr.imageCam.Source = (System.Windows.Media.ImageSource)ToBitmapImage(bitmap);
            else
                windowCurr.Dispatcher.Invoke(new SetImageDelegate(SetImageCam), new object[] { bitmap });
        }
    }
}
