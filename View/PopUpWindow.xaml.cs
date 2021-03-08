using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ControlPanel.View
{
    /// <summary>
    /// Логика взаимодействия для PopUpWindow.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        private DispatcherTimer timer;
        private string IdCard;
        public PopUpWindow(string Id)
        {
            InitializeComponent();
            this.Top = 10;
            this.Left = 20;
            this.WindowStyle = WindowStyle.None;
            IdCard = Id;
            MainGrid.Children.Add(new Label() { Content = IdCard, FontSize=30});
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Start();
        }
        private void timerTick(object sender, EventArgs e)
        {
            timer.Stop();
            this.Hide();
        }
    }
}
