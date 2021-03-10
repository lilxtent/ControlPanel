using ControlPanel.Model;
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
            Content = new ClientModelInfo(ClientData);
            Margin = new Thickness(1);
            BorderBrush = Brushes.Black;
            BorderThickness = new Thickness(1);
        }
    }
}
