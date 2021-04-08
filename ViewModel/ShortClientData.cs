using ControlPanel.Model;
using ControlPanel.Sourses;
using System.Windows;
using System.Windows.Controls;

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
