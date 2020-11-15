using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenMate
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        protected double a = 0.5;
        protected double flyingTime = 1;
        protected Point location;
        protected Point cursorLocation;
        public int height = 100;
        public int width = 67;

        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();

            Mate.Height = height;
            Mate.Width = width;

            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Start();
            timer.Tick += TickTimer;

        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        private void TickTimer(object sender, EventArgs e)
        {
            location = PointToScreen(new Point(0, 0));
            cursorLocation = new Point(System.Windows.Forms.Control.MousePosition.X-22, System.Windows.Forms.Control.MousePosition.Y-38);
            double moveX = cursorLocation.X - location.X;
            double moveY = cursorLocation.Y - location.Y;
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Left += 5 * (moveX) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));

                if (Top < System.Windows.SystemParameters.PrimaryScreenHeight - 140)
                {
                    if (flyingTime < 0)
                    {
                        Top -= a * Math.Pow(flyingTime, 2) / 2;
                    }
                    else
                    {
                        Top += a * Math.Pow(flyingTime, 2) / 2;
                    }
                    flyingTime += 0.2;
                }
                else
                {
                    //Здесь он прыгает как только касается низа
                    Top = System.Windows.SystemParameters.PrimaryScreenHeight - 140;
                    flyingTime = -9;
                    Top -= a * Math.Pow(flyingTime, 2) / 2;
                }
            }
            else
            {
                flyingTime = 1;
            }
            //Это перемещение за курсорм 
            //Top += 3* (moveY) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
        }

    }


    
}
