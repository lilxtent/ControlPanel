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
            Background = ClientMethods.FindColor(ClientData);
            IsManipulationEnabled = false;
        }
       
    }
}
