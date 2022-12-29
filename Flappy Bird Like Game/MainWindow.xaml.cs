using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Runtime.InteropServices;
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

namespace Flappy_Bird_Like_Game
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ImageSource downObstacle = new BitmapImage(new Uri(@"DownObstacle.png", UriKind.Relative));
        public ImageSource upObstacle = new BitmapImage(new Uri(@"UpObstacle.png", UriKind.Relative));


        public bool Gamerunning { get; set; }
        public int Score { get; set; }
        public double Gamespeed { get; set; }
        public int BirdSpeed { get; set; }
        public bool Birdfalling { get; set; }
        public double BirdAngle { get; set; }

        public int ObstacleRate = 2000;

        public MainWindow()
        {
            InitializeComponent();
            Gamespeed = 2;
            BirdSpeed = 30;
            BirdAngle = 0;
            Gamerunning = true;
            Birdfalling = true;
            this.Background = Brushes.White;
            AddObstacle();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GameLoop();
        }

        public async void GameLoop()
        {
            int secondcount = 0;
            while ( Gamerunning )
            {
                await Task.Delay(15);
                
                secondcount += 15;

                BirdGravity();

                if ( secondcount >= ObstacleRate )
                {
                    AddObstacle();
                    secondcount = 0;
                }


                MoveObstacles();
                if (BirdHitWall())
                {
                    Gamerunning = false;
                    BirdFall();
                }

                

            }
            endScreen.Visibility = Visibility.Visible;
        }


        // OBSTACLE ==============
        public void AddObstacle()
        {
            if (Gamerunning) 
            {
                Random rand = new Random();
                int point = rand.Next(0+100, (int)gameCanvas.Height - 100);

                Rectangle rectangle1 = new Rectangle();
                Rectangle rectangle2 = new Rectangle();

                Canvas.SetZIndex(rectangle1, 2); 
                rectangle1.Width = upObstacle.Width * 2.5; rectangle2.Width = downObstacle.Width * 2.5;
                rectangle1.Height = upObstacle.Height * 2;
                rectangle2.Height = downObstacle.Height * 2;

                
                rectangle1.Fill = new ImageBrush(upObstacle);
                rectangle2.Fill = new ImageBrush(downObstacle);

               

                gameCanvas.Children.Add(rectangle1);
                gameCanvas.Children.Add(rectangle2);
                Canvas.SetTop(rectangle1, 0 - upObstacle.Height * 2 + point - 100);
                Canvas.SetLeft(rectangle1, gameCanvas.Width );
                Canvas.SetTop(rectangle2, point + 100);
                Canvas.SetLeft(rectangle2, gameCanvas.Width );

            }
        }

        public void MoveObstacles()
        {
            Score = 0;
            foreach ( Rectangle x in gameCanvas.Children.OfType<Rectangle>() )
            {
                Canvas.SetLeft(x, Canvas.GetLeft(x) - Gamespeed);
                if (Canvas.GetLeft(x) < Canvas.GetLeft(bird))
                {

                    Score++;
                    SystemSounds.Beep.Play();
                }

                if (Canvas.GetLeft(x) < 0 - x.Width)
                    x.Fill = Brushes.Transparent; 

            }
            ScoreLabel.Content = $"{Score/2}";
            
        }





        // BIRD CONTROLS =======
        public async void BirdJump()
        {
            Birdfalling = false;
            for ( int i=1; i <= 15; i++ )
            {
                await Task.Delay(1);
                MoveUp();
                if (BirdAngle > 0)
                    BirdAngle = 0;
                if (BirdAngle > -45)
                    BirdAngle -= 0.75;
                
                RotateTransform rotate = new RotateTransform(BirdAngle);
                bird.LayoutTransform = rotate;
            }
            if (Canvas.GetTop(bird) < 0)
                BirdFall();
            

            await Task.Delay(75);
            Birdfalling = true;
            
        }

        public async void BirdFall()
        {

            int fallangle = 0;
            Gamerunning = false;
            while (Canvas.GetTop(bird) < gameCanvas.Height - bird.Height  - 25)
            {
                await Task.Delay(25);
                Canvas.SetTop(bird, Canvas.GetTop(bird) + BirdSpeed);

                if (fallangle < 75)
                    fallangle += 5;

                RotateTransform rotate = new RotateTransform(fallangle);
                bird.LayoutTransform = rotate;
            }
        }

        public async void BirdGravity()
        {
            if (Birdfalling)
            {
                Canvas.SetTop(bird, Canvas.GetTop(bird) + 5);
                if (BirdAngle < 0)
                    BirdAngle += 2 ;
                if ( BirdAngle < 75 )
                {
                    BirdAngle += 1;
                    RotateTransform rotate = new RotateTransform(BirdAngle);
                    bird.LayoutTransform = rotate;
                }
                if (Canvas.GetTop(bird) > gameCanvas.Height - bird.Height )
                    Gamerunning = false;

            }
        }

        public void MoveUp()
        {
            Canvas.SetTop(bird, Canvas.GetTop(bird) - BirdSpeed/6);
        }

        public void MoveDown()
        {
            Canvas.SetTop(bird, Canvas.GetTop(bird) + BirdSpeed/6);
        }


        public bool BirdHitWall()
        {
            Rect birdHitbox = new Rect(Canvas.GetLeft(bird) + 10, Canvas.GetTop(bird) + 10, bird.Width -10 , bird.Height - 10  );
            
            foreach ( Rectangle x in gameCanvas.Children.OfType<Rectangle>() )
            {
                Rect hitbox = new Rect( Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height );
                if (birdHitbox.IntersectsWith(hitbox))
                    return true;
            }

            return false;
        }

        public void RestartGame()
        {
            MainWindow window = new MainWindow();
            window.Show();
            this.Hide();
        }



        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Gamerunning)
            {
                if (e.Key == Key.R)
                    RestartGame();
                else
                    return;
            }

           
            if (e.Key == Key.Space)
                BirdJump();

        }

        private void closeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}
