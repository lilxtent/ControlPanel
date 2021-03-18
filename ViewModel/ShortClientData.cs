using ControlPanel.Model;
using ControlPanel.Sourses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ControlPanel.ViewModel
{
    public class ListBoxShortClientData : ListBoxItem
    {
        /// <summary>
        /// Предназначен для отображения краткой информации о клиенте в элементе ListBox
        /// </summary>
        public ListBoxShortClientData(ClientModel ClientData)
        {
            Content = new ClientArea(ClientData).getGrid();
            Margin = new Thickness(1);
            Background = FindColor(ClientData);
            IsManipulationEnabled = false;
        }
        private SolidColorBrush FindColor(ClientModel ClientData)
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
    }
}
