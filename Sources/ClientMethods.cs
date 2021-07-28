using ControlPanel.Model;
using System;
using System.Windows.Media;
using System.Xml;

namespace ControlPanel.Sourses
{
    class ClientMethods
    {
        public ClientMethods() { }
        public static SolidColorBrush FindColor(ClientModel ClientData)
        {
            SolidColorBrush ColorBrush = new SolidColorBrush(Color.FromArgb(200, 196, 196, 194));
            // в случае если клиент ни разу не производил оплату
            if (ClientData.DateLastPayment == default)
            {
                return ColorBrush;
            }

            int diffDays = (ClientData.DateLastPayment - DateTime.Today).Days;
            // абонемент просрочен
            if (diffDays < 0)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 255, 112, 99));
            }
            // остался 1 день
            else if (diffDays <= 1)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 250, 155, 100));
            }
            // осталось 3 дня
            else if (diffDays <= 3)
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 242, 250, 100));
            }
            // осталось больше 3 дней
            else
            {
                ColorBrush = new SolidColorBrush(Color.FromArgb(200, 206, 255, 107));
            }
            return ColorBrush;
        }
        public static MediaPlayer FindSound(ClientModel ClientData)
        {
            MediaPlayer mediaPlayer = new ();
            // в случае если клиент ни разу не производил оплату
            if (ClientData.DateLastPayment == default)
            {
                mediaPlayer.Open(new Uri(ConvertRelativeToAbsolutePath(@"\Sounds\SoundUnpay.mp3")));
                return mediaPlayer;
            }

            int diffDays = (ClientData.DateLastPayment - DateTime.Today).Days;
            // абонемент просрочен
            if (diffDays < 0)
            {
                mediaPlayer.Open(new Uri(ConvertRelativeToAbsolutePath(@"\Sounds\SoundError.mp3")));

            }
            // остался 1 день
            else if (diffDays <= 1)
            {
                mediaPlayer.Open(new Uri(ConvertRelativeToAbsolutePath(@"\Sounds\SoundConfirm.mp3")));

            }
            // осталось 3 дня
            else if (diffDays <= 3)
            {
                mediaPlayer.Open(new Uri(ConvertRelativeToAbsolutePath(@"\Sounds\SoundConfirm.mp3")));

            }
            // осталось больше 3 дней
            else
            {
                mediaPlayer.Open(new Uri(ClientMethods.ConvertRelativeToAbsolutePath(@"\Sounds\SoundConfirm.mp3")));
            }
            return mediaPlayer;
        }
        public static SolidColorBrush GetRedColorBrush() =>
            new SolidColorBrush(Color.FromArgb(200, 255, 112, 99));
        public static SolidColorBrush GetGreenColorBrush() =>
            new SolidColorBrush(Color.FromArgb(200, 206, 255, 107));

        public static string ConvertRelativeToAbsolutePath(string pathRelative) =>
            Environment.CurrentDirectory + pathRelative;

        public static string GetOnlyFileName(string path)
        {
            string[] pathSplittedArr = path.Split(@"\");
            return pathSplittedArr[pathSplittedArr.Length - 1];
        }
        public static string GetDefaultImagePathRelative() => @"\Photos\default-user-image.png";
        public static string GetDefaultImagePathAbsolute() =>
            ConvertRelativeToAbsolutePath(@"\Photos\default-user-image.png");
        public static string GetCameraNameConfig()
        {
            string cameraNameInConfig = "";
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(ConvertRelativeToAbsolutePath(@"\Config.xml"));
            foreach (XmlNode xNode in xDoc.ChildNodes)
                if (xNode.Name == "Camera")
                    cameraNameInConfig = xNode.InnerText;
            return cameraNameInConfig;
        }

    }
}
