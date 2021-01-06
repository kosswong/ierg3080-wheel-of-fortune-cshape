using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace L6
{
    public partial class MainWindow : Window
    {
        static int wheel_divided = 8;
        int money = 1000;
        double status; //0=no; 1= starting; 2=ending
        Random rand = new Random();
        Path[] path = new Path[wheel_divided]; 
        TextBlock[] textBlocks = new TextBlock[wheel_divided];
        Dictionary<Color, int> Award = new Dictionary<Color, int>()
        {
            { Colors.Red, 100 },            //Red
            { Colors.LightSteelBlue, 10 },       //Blue
            { Colors.Aquamarine, 0 },       //Green
            { Colors.Gray, -500 },          //Gray
            { Colors.PeachPuff, 777 },
            { Colors.LightPink, -50 },
            { Colors.LightYellow, 666 },
        };
        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            InitializeAdward();
            UpdateWallet();
            start_button.IsEnabled = true;
            stop_button.IsEnabled = false;
        }
        private void InitializeAdward()
        {
            // Initialize the wheel
            List<int> temp_award_allcate = new List<int>();
            for (int i = 0; i < wheel_divided; i++)
            {
                path[i] = pies.Children[2 * i] as Path;
                textBlocks[i] = pies.Children[2*i+1] as TextBlock;
            }
            // Fill adward information to the wheel
            for (int i = 0, rdm; i < wheel_divided; i++)
            {
                do  {
                    rdm = rand.Next(Award.Count);
                } while (temp_award_allcate.Contains(rdm) && temp_award_allcate.Count < Award.Count);
                temp_award_allcate.Add(rdm);
                // Update color and text of award
                path[i].Fill = new SolidColorBrush(Award.Keys.ElementAt(rdm));
                textBlocks[i].Text = Award.Values.ElementAt(rdm).ToString();
            }
        }
        // Every second past, trigger this call
        private void DispatcherTimerTick(object sender, EventArgs e)
        {
            // Decrease acceleration when status set to 2 or above, float is to increase the randomness as it prevent returning a constant
            if (status >= 2) status += 0.03 * (1 + status * status * 0.01);
            if (status > 200){
                StopWheelAndGetAdward();
            } else {
                ROTATE.Angle = (ROTATE.Angle + 30.34 / status) % 360;
            }
        }
        private void UpdateWallet(int amountToChange = 0)
        {
            money += amountToChange;
            textBlock.Text = "You have:\n" + money.ToString("C");
        }
        private void StartClick(object sender, RoutedEventArgs e)
        {
            status = 1;
            start_button.Foreground = new SolidColorBrush(Colors.Transparent);

            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1);
            dispatcherTimer.Start();

            start_button.IsEnabled = false;
            stop_button.IsEnabled = true;
            stop_button.Foreground = new SolidColorBrush(Colors.White);
        }
        private void StopClick(object sender, RoutedEventArgs e)
        {
            status = 2;
            stop_button.Foreground = new SolidColorBrush(Colors.Transparent);
            stop_button.IsEnabled = false;
        }
        private void StopWheelAndGetAdward()
        {
            // Step the timer
            dispatcherTimer.Tick -= DispatcherTimerTick;
            dispatcherTimer.Stop();

            // Calculate the arward by degree of rotation of the circle
            // There are totally 8 award
            // float award = 8*(degree/360)
            // for example 
            // 0 - 0.99 = award 0
            // 1 - 1.99 = award 1
            // 2 - 2.99 = award 2 etc.
            int awardClass = (int)Math.Floor(wheel_divided * ROTATE.Angle / 360);
            int awardAmount = int.Parse(textBlocks[awardClass].Text);
            MessageBox.Show("You " + (awardAmount >= 0 ? "are lucky, you obtained " : "are unlucky, you wallet just ") + awardAmount + "!", "Thank you!");
            UpdateWallet(awardAmount);

            // Allow to start the game again
            start_button.Foreground = new SolidColorBrush(Colors.White);
            start_button.IsEnabled = true;
        }
    }
}
