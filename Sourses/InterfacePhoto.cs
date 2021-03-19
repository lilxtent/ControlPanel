using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;


namespace ControlPanel.Sourses
{
    interface IPhoto<T>
    {
        Image getImageConteiner();
        TextBox getIdConteiner();

    }
}
