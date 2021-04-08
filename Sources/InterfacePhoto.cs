using System.Windows.Controls;


namespace ControlPanel.Sourses
{
    interface IPhoto<T>
    {
        Image getImageConteiner();
        TextBox getIdConteiner();
        bool getChangedCameraStatus();
        void setChangedCameraStatus(bool flag);

    }
}
