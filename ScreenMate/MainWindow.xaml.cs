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
        double deltaMouse = 25;
        //Переменные перемещения позиции окна за один такт
        double moveX, moveY;
        double moveMouseX, moveMouseY;
        bool kick = false;
        //Коэффициент перемещения (для определения, вверх или вниз перемещается персонаж)
        int koeff;
        //Инициализация таймеров
        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer timerChangeMode = new System.Windows.Threading.DispatcherTimer();
        //Режим зверька
        int mateMode = 2;
        //Инициализация спрайтовв
        BitmapImage ball = new BitmapImage(new Uri("image/ball.png", UriKind.Relative));
        BitmapImage benis = new BitmapImage(new Uri("image/benis.png", UriKind.Relative));
        BitmapImage vortex = new BitmapImage(new Uri("image/vortex.png", UriKind.Relative));

        /// <summary>
        /// Функция создания окна
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            //Установка размеров окна
            Mate.Height = height + 20;
            Mate.Width = width + 20;
            Mate.MaxHeight = height + 20;
            Mate.MaxWidth = width + 20;
            //Стартовый зверь
            img.Source = benis;
            //Установка интервала действия персонажа
            timer.Interval = TimeSpan.FromSeconds(0.01);
            timer.Start();
            timer.Tick += TickerTimerBenis;
            timer.Tick += ControlWindowSize;
            //Таймер на смену режима
            timerChangeMode.Interval = TimeSpan.FromSeconds(25);
            timerChangeMode.Start();
            timerChangeMode.Tick += TimerChangeMode;

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
        /// Логика засасывающей воронки
        /// </summary>
        /// <param name="sender">Объект-центр всасывания</param>
        /// <param name="e">Событие для таймера</param>
        private void TickerTimerVortex(object sender, EventArgs e)
        {
            location = PointToScreen(new Point(0, 0));
            cursorLocation = new Point(System.Windows.Forms.Control.MousePosition.X - 22, System.Windows.Forms.Control.MousePosition.Y - 38);
            //Вектор от воронки до курсора
            moveX = cursorLocation.X - (location.X + width / 2 - 14);
            moveY = cursorLocation.Y - (location.Y + height / 2 - 25);
            rotate.Angle += 5;
            //Перемещение курсора к воронке
            MoveMouse(-5 * (moveX) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2)), -5 * (moveY) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2)));
        }

        /// <summary>
        /// Логика перемещения бениса
        /// </summary>
        /// <param name="sender">Объект перемещения</param>
        /// <param name="e">Событие для таймера</param>
        private void TickerTimerBenis(object sender, EventArgs e)
        {
            location = PointToScreen(new Point(0, 0));
            cursorLocation = new Point(System.Windows.Forms.Control.MousePosition.X - 22, System.Windows.Forms.Control.MousePosition.Y - 38);
            //Вектор от бениса до курсора
            moveX = cursorLocation.X - (location.X + width / 2 - 14);
            moveY = cursorLocation.Y - (location.Y + height / 2 - 25);
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                //Перемещение бениса к курсору  
                Top += 5 * (moveY) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
                Left += 5 * (moveX) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
                //Логика поворота
                if (moveX < 0)
                {
                    scale.CenterX = width / 2;
                    scale.ScaleX = -1;
                }
                else
                {
                    scale.CenterX = 0;
                    scale.ScaleX = 1;
                }
                //Отталкивание курсора
                Kicking();
            }
        }

        /// <summary>
        /// Логика перемещения мяча
        /// </summary>
        /// <param name="sender">Объект перемещения</param>
        /// <param name="e">Событие для таймера</param>
        private void TickTimerBall(object sender, EventArgs e)
        {
            location = PointToScreen(new Point(0, 0));
            cursorLocation = new Point(System.Windows.Forms.Control.MousePosition.X - 22, System.Windows.Forms.Control.MousePosition.Y - 38);
            //Вектор от мяча до курсора
            moveX = cursorLocation.X - (location.X + width / 2 - 14);
            moveY = cursorLocation.Y - (location.Y + height / 2 - 25);
            //Коэффициент направления перемещения
            koeff = 1;

            //Если левая клавиша не зажата (т.е. животное не перетаскивается)
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                Kicking();
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
                    flyingTime = -11;
                    koeff = -1;
                    Top += koeff * a * Math.Pow(flyingTime, 2) / 2;
                }
            }
            else
            {
                flyingTime = 1;
            }
        }

        /// <summary>
        /// Метод пинка курсора мыши в зависимости от положения объекта и положения курсора
        /// </summary>
        private void Kicking()
        {
            if (kick)
            {
                MoveMouse(moveMouseX, moveMouseY);
                moveMouseX /= 2;
                moveMouseY /= 2;
                if (moveMouseX <= 1 && moveMouseY <= 1)
                {
                    kick = false;
                }
            }
            if ((location.X + width / 2 + deltaMouse > cursorLocation.X) && (location.X + width / 2 - deltaMouse - 35 < cursorLocation.X) && (location.Y + height / 2 - 20 + deltaMouse + 10 > cursorLocation.Y) && (location.Y + height / 2 - 50 - deltaMouse < cursorLocation.Y))
            {
                kick = true;
                moveMouseX = 105 * (moveX) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
                moveMouseY = 105 * (moveY) / Math.Sqrt(Math.Pow(moveX, 2) + Math.Pow(moveY, 2));
            }
        }

        /// <summary>
        /// Метод перемещения курсора мыши на определенное расстояние (с обработкой исключения)
        /// </summary>
        /// <param name="dx">Перемещение по оси X</param>
        /// <param name="dy">Перемещение по оси Y</param>
        private void MoveMouse(double dx, double dy)
        {
            try
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Convert.ToInt32(System.Windows.Forms.Cursor.Position.X + dx), Convert.ToInt32(System.Windows.Forms.Cursor.Position.Y + dy));
            }
            catch
            {
                System.Windows.Forms.Cursor.Position = new System.Drawing.Point(Convert.ToInt32(System.Windows.Forms.Cursor.Position.X - 100), Convert.ToInt32(System.Windows.Forms.Cursor.Position.Y - 100));
            }
        }
        /// <summary>
        /// Метод смены режима персонажа через некоторое время
        /// </summary>
        /// <param name="sender">Объект персонажа</param>
        /// <param name="e">Эвент для таймера</param>
        private void TimerChangeMode(object sender, EventArgs e)
        {
            timerChangeMode.Interval = TimeSpan.FromSeconds(0.01);
            //Снятие всех режимов
            timer.Tick -= TickTimerBall;
            timer.Tick -= TickerTimerBenis;
            timer.Tick -= TickerTimerVortex;
            scale.ScaleX = 1;
            //Смещение центра (для вращения)
            rotate.CenterX = (width + 22) / 2;
            rotate.CenterY = (height) / 2;
            if (Mouse.LeftButton != MouseButtonState.Pressed)
            {
                //Если прошло достаточно времени поворота - смена режима
                if (flyingTime >= 120)
                {
                    timerChangeMode.Interval = TimeSpan.FromSeconds(25);
                    flyingTime = 0;
                    rotate.Angle = 0;
                    //Выбор режима
                    mateMode = new Random().Next(0, 3);
                    if (mateMode == 0)
                    {
                        timer.Tick += TickerTimerVortex;
                        img.Source = vortex;

                    }
                    else if (mateMode == 1)
                    {
                        timer.Tick += TickTimerBall;
                        img.Source = ball;
                    }
                    else
                    {
                        timer.Tick += TickerTimerBenis;
                        img.Source = benis;
                    }
                }
                else
                {
                    flyingTime += 1;
                    Kicking();
                }
            }
            rotate.Angle += flyingTime;

        }

        /// <summary>
        /// Проверка расширения экрана
        /// </summary>
        /// <param name="sender">Объект расширения</param>
        /// <param name="e">Событие для таймера</param>
        private void ControlWindowSize(object sender, EventArgs e)
        {
            //Контроль расширения (для восстановления)
            if (Mate.WindowState != WindowState.Normal)
            {
                Mate.WindowState = WindowState.Normal;
            }
        }
    }


    
}
