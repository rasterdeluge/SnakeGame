using Windows.UI;
using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Microsoft.Graphics.Canvas.Effects;
using System;
using System.Security;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;

namespace SnakeGame
{
    public static class ListExtensions
    {

        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }

            return source;
        }
    }
    public sealed partial class MainPage : Page
    {
        Random rnd = new Random();
        int playerXaxis = 250, playerYaxis = 250;
        public bool[] keys = new bool[] { false, false, false, false, false };
        public const int up = 0, down = 1, left = 2, right = 3, space = 4;
        public const int intro = 0, mainMenu = 1, about = 2, snake = 3, gameOver = 4;
        int[] arrayX = new int[2000];
        int[] arrayY = new int[2000];
        int[] arrayXTemp = new int[2000];
        int[] arrayYTemp = new int[2000];
        public static ICollection<string> lines = new SortedSet<string>();
        public static ICollection<int> lines2 = new SortedSet<int>();
        public static ICollection<int> lines3 = new SortedSet<int>();
        int foodAmount = -1;
        int foodX = 0;
        int foodY = 0;
        bool food = false;
        int prevKey = 20;
        int state = 1;
        byte intro_color = 100;
        int menu_selector = 0;

        static TimeSpan TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 20.0);

        public MainPage()
        {
            this.InitializeComponent();
        }

        void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown; // KeyDown_UIThread;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp; // KeyDown_UIThread;
        }

        private void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender,
                                        Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Up:
                    keys[up] = true;
                    keys[down] = false;
                    keys[left] = false;
                    keys[right] = false;
                    keys[space] = false;
                    break;
                case VirtualKey.Down:
                    keys[up] = false;
                    keys[down] = true;
                    keys[left] = false;
                    keys[right] = false;
                    keys[space] = false;
                    break;
                case VirtualKey.Left:
                    keys[up] = false;
                    keys[down] = false;
                    keys[left] = true;
                    keys[right] = false;
                    keys[space] = false;
                    break;
                case VirtualKey.Right:
                    keys[up] = false;
                    keys[down] = false;
                    keys[left] = false;
                    keys[right] = true;
                    keys[space] = false;
                    break;
                case VirtualKey.Space:
                    keys[space] = true;
                    break;
            }
            args.Handled = true; // tell the system we handled this event.
        }
        private void CoreWindow_KeyUp(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs args)
        {
            switch (args.VirtualKey)
            {
                case VirtualKey.Space:
                    keys[space] = false;
                    break;
            }
            args.Handled = true; // tell the system we handled this event.
        }

        private void canvas_CreateResources(
            Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender,
            Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                canvas.Width = 600;
                canvas.Height = 600;
                canvas.TargetElapsedTime = TargetElapsedTime;
            }
        }

        private void canvas_AnimatedDraw(
    Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender,
    Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            if (state == snake)
            {
                // background
                args.DrawingSession.FillRectangle(0, 0, 600, 600, Color.FromArgb(255, 100, 100, 100));
                // background
                args.DrawingSession.FillRectangle(200, 200, 200, 200, Color.FromArgb(255, 0, 0, 0));
                // snake head
                args.DrawingSession.FillRectangle(playerXaxis, playerYaxis, 4, 4, Color.FromArgb(255, 0, 200, 0));
                // snake body
                for (int i = 0; i <= foodAmount; i++)
                {
                    args.DrawingSession.DrawRectangle((arrayX[i]), (arrayY[i]), 4, 4, Color.FromArgb(255, 0, 255, 0));
                }
                // borders
                args.DrawingSession.FillRectangle(200, 200, 4, 200, Color.FromArgb(255, 0, 255, 0));
                args.DrawingSession.FillRectangle(200, 200, 200, 4, Color.FromArgb(255, 0, 255, 0));
                args.DrawingSession.FillRectangle(200, 396, 200, 4, Color.FromArgb(255, 0, 255, 0));
                args.DrawingSession.FillRectangle(396, 200, 4, 200, Color.FromArgb(255, 0, 255, 0));
                // food
                args.DrawingSession.FillRectangle(foodX, foodY, 4, 4, Color.FromArgb(255, 255, 0, 0));
                // score
                string foodAmountString = (foodAmount + 1).ToString();
                args.DrawingSession.DrawText("Score:", 300, 175, Colors.PowderBlue);
                args.DrawingSession.DrawText(foodAmountString, 370, 175, Colors.PowderBlue);
            }
            if (state == intro)
            {

                // background
                args.DrawingSession.FillRectangle(0, 0, 600, 600, Color.FromArgb(intro_color, 0, 255, 0));
                args.DrawingSession.DrawText("Welcome to Snake C# 0.01v", 180, 280, Colors.Blue);
                intro_color++;
                if (intro_color == 255)
                {
                    state = mainMenu;
                }
            }
            if (state == about)
            {
                {
                    // background
                    args.DrawingSession.FillRectangle(0, 0, 600, 600, Color.FromArgb(255, 100, 100, 100));
                    args.DrawingSession.DrawText("Created By", 240, 240, Colors.Blue);
                    args.DrawingSession.DrawText("Cooked Kraken", 240, 260, Colors.Blue);
                    args.DrawingSession.DrawText("1/31/2017", 240, 280, Colors.Blue);
                    args.DrawingSession.DrawText("11AM", 240, 300, Colors.Blue);
                }
            }
            if (state == gameOver)
            {
                {
                    Windows.Storage.StorageFolder installedLocation = Windows.Storage.ApplicationData.Current.LocalFolder;

                    // background
                    args.DrawingSession.FillRectangle(0, 0, 600, 600, Color.FromArgb(255, 0, 0, 0));
                    args.DrawingSession.DrawText("Game Over", 240, 240, Colors.Red);

                    var scoresFilePath = Path.Combine(installedLocation.Path, "scores.txt");
                    //var scoresFilePath = Path.Combine(installedLocation.Path, "scores.text");
                    // score
                    string foodAmountString = (foodAmount + 1).ToString();
                    if (System.IO.File.ReadAllLines(scoresFilePath) != null)
                    {
                        foreach (var item in System.IO.File.ReadAllLines(scoresFilePath))
                        {
                            lines.Add(item); // get each score
                        }
                    }

                    lines.Add(foodAmountString); // add current score
                    foreach (var val in lines) // copy the string list to an int list
                    {
                        lines2.Add(Int32.Parse(val));
                    }
                    foreach (var val in lines2.Reverse())// lines3 = lines2 reversed, shortened to 10
                    {
                        lines3.Add(val);
                        if (lines3.LongCount() >= 10)
                            break;
                    }
                    if (lines2.LongCount() >= 10)
                    {
                        lines2.Clear();
                        foreach (var val in lines3.Reverse())
                        {
                            lines2.Add(val);
                            if (lines2.LongCount() >= 10)
                                break;
                        }
                    }
                    if (lines3.LongCount() >= 10)
                    {
                        lines3.Clear();
                        foreach (var val in lines2.Reverse())
                        {
                            lines3.Add(val);
                            if (lines3.LongCount() >= 10)
                                break;
                        }
                    }

                    System.IO.File.WriteAllLines(scoresFilePath, lines);
                    lines.Clear();

                    args.DrawingSession.DrawText("Score:", 240, 200, Colors.PowderBlue);
                    args.DrawingSession.DrawText(foodAmountString, 310, 200, Colors.PowderBlue);

                    (lines3.Reverse()).Where(x => x != 0).Each(matthew => args.DrawingSession.DrawText(matthew.ToString(), 240, 280 + (20 * Array.FindIndex((lines3.Reverse()).ToArray(), y => matthew == y) + 1), Colors.Blue));
                }
            }
            if (state == mainMenu)
            {
                // background
                args.DrawingSession.FillRectangle(0, 0, 600, 600, Color.FromArgb(255, 0, 0, 0));
                args.DrawingSession.DrawText("Start Snake", 240, 240, Colors.Blue);
                args.DrawingSession.DrawText("High Score", 240, 260, Colors.Blue);
                args.DrawingSession.DrawText("My Options", 240, 280, Colors.Blue);
                args.DrawingSession.DrawText("About Game", 240, 300, Colors.Blue);

                if (menu_selector == 0)
                    args.DrawingSession.FillRectangle(230, 245, 130, 20, Color.FromArgb(100, 0, 0, 150));

                if (menu_selector == 1)
                    args.DrawingSession.FillRectangle(230, 265, 130, 20, Color.FromArgb(100, 0, 0, 150));

                if (menu_selector == 2)
                    args.DrawingSession.FillRectangle(230, 285, 130, 20, Color.FromArgb(100, 0, 0, 150));

                if (menu_selector == 3)
                    args.DrawingSession.FillRectangle(230, 305, 130, 20, Color.FromArgb(100, 0, 0, 150));
            }
        }

        private void canvas_AnimatedUpdate(
    Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender,
    Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedUpdateEventArgs args)
        {
            if (state == intro)
                Intro();
            if (state == mainMenu)
                MainMenu();
            if (state == about)
                About();
            if (state == snake)
                Snake_Logic();
            if (state == gameOver)
                GameOver();
        }
        void Intro()
        {
            if (keys[space])
                state = mainMenu;
        }
        void Snake_Logic()
        {
            //food collision
            if ((playerXaxis > foodX - 4 && playerXaxis < foodX + 4)
                    && (playerYaxis > foodY - 4 && playerYaxis < foodY + 4))
            {
                food = false;
                foodAmount++;
            }
            //food generator
            if (food == false)
            {
                food = true;
                foodX = rnd.Next(210, 390);
                foodY = rnd.Next(210, 390);
            }

            // Snake Body Array
            arrayX[0] = playerXaxis;
            arrayY[0] = playerYaxis;
            for (int i = 0; i <= foodAmount; i++)
            {
                arrayX[i + 1] = arrayXTemp[i];
                arrayY[i + 1] = arrayYTemp[i];
            }
            for (int i = 0; i <= foodAmount; i++)
            {
                arrayXTemp[i] = arrayX[i];
                arrayYTemp[i] = arrayY[i];
            }
            // Snake Controls
            if (keys[up])
            {
                if (prevKey != down)
                {
                    playerYaxis -= 4;
                    prevKey = up;
                }
                else
                    playerYaxis += 4;
            }
            else if (keys[down])
            {
                if (prevKey != up)
                {
                    playerYaxis += 4;
                    prevKey = down;
                }
                else
                    playerYaxis -= 4;
            }
            if (keys[left])
            {
                if (prevKey != right)
                {
                    playerXaxis -= 4;
                    prevKey = left;
                }
                else
                    playerXaxis += 4;
            }
            else if (keys[right])
            {
                if (prevKey != left)
                {
                    playerXaxis += 4;
                    prevKey = right;
                }
                else
                    playerXaxis -= 4;
            }
            if (keys[space])
            {
                foodAmount++;
            }
            if (playerXaxis < 204)
                playerXaxis = 204;
            if (playerXaxis > 392)
                playerXaxis = 392;
            if (playerYaxis < 204)
                playerYaxis = 204;
            if (playerYaxis > 392)
                playerYaxis = 392;
            for (int i = 0; i <= foodAmount; i++)
            {
                if (arrayX[i] == playerXaxis && arrayY[i] == playerYaxis)
                {
                    state = gameOver;
                }
            }
        }

        void About()
        {
            if (keys[space])
            {
                state = mainMenu;
            }
        }

        void GameOver()
        {
            int selectorX = 0, selectorY = 0;
            if (selectorX < 0)
                selectorX = 5;
            if (selectorX > 5)
                selectorX = 0;
            if (selectorY > 5)
                selectorY = 0;
            if (keys[space])
            {
                state = mainMenu;
            }
        }

        void MainMenu()
        {
            if (menu_selector < 0)
                menu_selector = 3;
            if (menu_selector > 3)
                menu_selector = 0;
            if (keys[up])
            {
                menu_selector--;
                keys[up] = false;
            }
            else if (keys[down])
            {
                menu_selector++;
                keys[down] = false;
            }
            else if (menu_selector == 0 && keys[space])
            {
                state = snake;
                keys[space] = false;
                foodAmount = -1;
            }
            else if (menu_selector == 1 && keys[space])
            {
                //state = HIGHSCORE;
                keys[space] = false;
            }
            else if (menu_selector == 2 && keys[space])
            {
                //state = MYOPTIONS;
                keys[space] = false;
            }
            else if (menu_selector == 3 && keys[space])
            {
                state = about;
                keys[space] = false;
            }
        }

        void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            this.canvas.RemoveFromVisualTree();
            this.canvas = null;
        }
    }
}
