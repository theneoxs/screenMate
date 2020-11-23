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
        //Ускорение свободного падения
        protected double a = 0.5;
        //Время в воздухе
        protected double flyingTime = 1;
        //Точка расположения животного
        protected Point location;
        //Точка расположения курсора
        protected Point cursorLocation;
        //Высота животного
        public int height = 100;
        //Ширина животного
        public int width = 67;
        //Поле, в котором происходит триггер курсора
        double deltaMouse = 10;
        //Переменные перемещения позиции окна за один такт
        double moveX, moveY;
        double moveMouseX, moveMouseY;
        bool kick = false;
        //Коэффициент перемещения (для определения, вверх или вниз перемещается персонаж)
        int koeff;
        //Инициализация таймера
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();

        /// <summary>
        /// Функция создания окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Установка размеров окна
            Mate.Height = height;
            Mate.Width = width;
            //Установка интервала перемещения окна (в мс)
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Start();
            //Добавление функции на завершение времени таймера
            timer.Tick += TickTimer;

        }

        /// <summary>
        /// Метод перемещения окна при зажатой левой кнопки мыши
        /// </summary>
        /// <param name="e">Событие нажатия кнопки мыши</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        /// <summary>
        /// Метод перемещения окна по экрану
        /// </summary>
        /// <param name="sender">Объект перемещения</param>
        /// <param name="e">Событие окончания времени (для таймера)</param>
        private void TickTimer(object sender, EventArgs e)
        { 
            location = PointToScreen(new Point(0, 0));
            cursorLocation = new Point(System.Windows.Forms.Control.MousePosition.X-22, System.Windows.Forms.Control.MousePosition.Y-38);
            moveX = cursorLocation.X - location.X;
            moveY = cursorLocation.Y - location.Y;
            koeff = 1;
            

            if (kick)
            {
                this.MoveMouse(moveMouseX, -moveMouseY);
                moveMouseX -= 1;
                moveMouseY -= 1;
                if (moveMouseX == 0 && moveMouseY == 0)
                {
                    kick = false;
                }
            }
            if (location.X + deltaMouse > cursorLocation.X && location.X - deltaMouse < cursorLocation.X && location.Y + deltaMouse > cursorLocation.Y && location.Y - deltaMouse < cursorLocation.Y)
            {
                kick = true;
                moveMouseX = 30;
                moveMouseY = 30;

            }
            //Если левая клавиша не зажата (т.е. животное не перетаскивается)
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Left += 5 * (moveX) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
                //Проверка, в воздухе ли животное
                if (Top < System.Windows.SystemParameters.PrimaryScreenHeight - 140)
                {
                    //Здесь он взлетает или падает, в зависимости от flyingTime
                    if (flyingTime < 0)
                    {
                        koeff = -1;
                    }
                    else
                    {
                        koeff = 1;
                    }
                    Top += koeff * a * Math.Pow(flyingTime, 2) / 2;
                    flyingTime += 0.2;
                }
                else
                {
                    //Здесь он прыгает как только касается низа (получает импульс)
                    Top = System.Windows.SystemParameters.PrimaryScreenHeight - 140;
                    flyingTime = -9;
                    koeff = -1;
                    Top += koeff * a * Math.Pow(flyingTime, 2) / 2;
                }
            }
            else
            {
                flyingTime = 1;
            }
            
            
            //Это перемещение за курсорм 
            //Top += 3* (moveY) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
        }

        /// <summary>
        /// Метод перемещения курсора мыши на определенное расстояние
        /// </summary>
        /// <param name="dx">Перемещение по оси X</param>
        /// <param name="dy">Перемещение по оси Y</param>
        private void MoveMouse(double dx, double dy)
        {
            System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Convert.ToInt32(System.Windows.Forms.Cursor.Position.X + dx), Convert.ToInt32(System.Windows.Forms.Cursor.Position.Y + dy));
        }

    }


    
}
