﻿using ControlPanel.Model;
using ControlPanel.Services;
using ControlPanel.Sourses;
using System;
using System.Media;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

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
        private MediaPlayer mediaPlayer { get; set; }

        public PopUpWindow(ClientModel Client, CameraModel Camera)
        {


            this.Client = Client;
            this.Camera = Camera;
            mediaPlayer = new();
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
            // если клиент найден выводим инфрмацию о нем
            if (Client is not null)
            {

                labelFIO.Content = $"{Client.Surname} {Client.Name} {Client.Patronymic}";
                Uri UriPath = new Uri(ClientMethods.GetDefaultImagePathAbsolute());
                if (File.Exists(ClientMethods.ConvertRelativeToAbsolutePath(Client.PhotoPath)))
                    UriPath = new Uri(ClientMethods.ConvertRelativeToAbsolutePath(Client.PhotoPath), UriKind.Absolute);

                imagePhoto.Source = new BitmapImage(UriPath);
                this.Background = ClientMethods.FindColor(Client);
                // звук отметки
                mediaPlayer = ClientMethods.FindSound(Client); 
                mediaPlayer.Play();
                SaveVisitInDB(); // сохраняем информацию о посещении
            }
            // иначе информируем об обратном
            else
            {
                imagePhoto.Source = new BitmapImage(new Uri(ClientMethods.GetDefaultImagePathAbsolute()));
                this.Background = new SolidColorBrush(Color.FromArgb(255, 254, 244, 137));
            }
        }

        private void SaveVisitInDB()
        {
            // мы считаем разницу между текущим временем и временем посещения только в случе если клиент
            // уже посещал клуб
            if (Client.DateLastVisit != default)
            {
                double diff = (DateTime.Now - Client.DateLastVisit).TotalSeconds;
                if (diff <= 1) return; // если в течении предыдущей минуты вы уже отмечались, то вас не запишет снова
            }
            ApplicationContext DB = new();
            DB.Visits.Add(new VisitModel(Client.ID, DateTime.Now));
            ClientModel NewClient = Client;
            NewClient.DateLastVisit = DateTime.Now;
            DB.ClientsModels.Update(NewClient);
            DB.SaveChanges();
        }
    }
}
