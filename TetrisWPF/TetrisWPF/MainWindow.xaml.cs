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
using System.Windows.Threading;

namespace TetrisWPF
{
    public class Board
    {
        int Rows;
        int Columns;
        int Score;
        int Linesfilled;
        Terminate currentTerminate;
        Label[,] BlockControls;

        static Brush NoBrush = Brushes.Transparent;
        static Brush SilverBrush = Brushes.Gray;
        public Board(Grid TetrisGrid)
        {
            Rows = TetrisGrid.RowDefinitions.Count;
            Columns = TetrisGrid.ColumnDefinitions.Count;
            Score = 0;
            Linesfilled = 0;

            BlockControls = new Label[Columns, Rows];
            for (int i = 0; i < Columns; i++)
            {
                for (int j = 0; j < Rows; j++)
                {
                    BlockControls[i, j] = new Label();
                    BlockControls[i, j].Background = NoBrush;
                    BlockControls[i, j].BorderBrush = SilverBrush;
                    BlockControls[i, j].BorderThickness = new Thickness(1, 1, 1, 1);
                    Grid.SetRow(BlockControls[i, j], j);
                    Grid.SetColumn(BlockControls[i, j], i);
                    TetrisGrid.Children.Add(BlockControls[i, j]);
                }
            }
            currentTerminate = new Terminate();
            currentTerminateDraw();
        }
        public int GetScore { get { return Score; } }
        public int GetLines { get { return Linesfilled; } }
        private void currentTerminateDraw()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] Shape = currentTerminate.getCurrentShape;
            Brush Color = currentTerminate.getCurrentColor;
            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + Position.X) + ((Columns / 2) - 1),
                    (int)(S.Y + Position.Y) + 2].Background = Color;
            }
        }
        private void currentTerminateErase()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] Shape = currentTerminate.getCurrentShape;
            Brush Color = currentTerminate.getCurrentColor;
            foreach (Point S in Shape)
            {
                BlockControls[(int)(S.X + Position.X) + ((Columns / 2) - 1),
                    (int)(S.Y + Position.Y) + 2].Background = NoBrush;
            }
        }
        private void CheckRows()
        {
            bool full;
            for (int i = Rows - 1; i > 0; i--)
            {
                full = true;
                for (int j = 0; j < Columns; j++)
                {
                    if (BlockControls[j, i].Background == NoBrush)
                    {
                        full = false;
                    }
                }
                if (full)
                {
                    RemoveRow(i);
                    Score += 100;
                    Linesfilled += 1;
                }
            }
        }
        private void RemoveRow(int row)
        {
            for (int i = row; i > 2; i--)
            {
                for (int j = 0; j < Columns; j++)
                {
                    BlockControls[j, i].Background = BlockControls[j, i - 1].Background;
                }
            }
        }
        public void CurrentTerminateMoveLeft()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] Shape = currentTerminate.getCurrentShape;
            bool move = true;
            currentTerminateErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Columns / 2) - 1) - 1) < 0)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Columns / 2) - 1) - 1),
                    (int)(S.Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currentTerminate.moveLeft();
                currentTerminateDraw();
            }
            else
            {
                currentTerminateDraw();
            }
        }
        public void CurrentTerminateMoveRight()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] Shape = currentTerminate.getCurrentShape;
            bool move = true;
            currentTerminateErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.X + Position.X) + ((Columns / 2) - 1) + 1) >= Columns)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + ((Columns / 2) - 1) + 1),
                    (int)(S.Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currentTerminate.moveRight();
                currentTerminateDraw();
            }
            else
            {
                currentTerminateDraw();
            }
        }
        public void CurrentTerminateMoveDown()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] Shape = currentTerminate.getCurrentShape;
            bool move = true;
            currentTerminateErase();
            foreach (Point S in Shape)
            {
                if (((int)(S.Y + Position.Y) + 2 + 1) >= Rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S.X + Position.X) + (Columns / 2) - 1),
                    (int)(S.Y + Position.Y) + 2 + 1].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currentTerminate.moveDown();
                currentTerminateDraw();
            }
            else
            {
                currentTerminateDraw();
                CheckRows();
                currentTerminate = new Terminate();
            }
        }
        public void CurrentTerminateRotate()
        {
            Point Position = currentTerminate.getCurrentPosition;
            Point[] S = new Point[4];
            Point[] Shape = currentTerminate.getCurrentShape;
            bool move = true;
            Shape.CopyTo(S, 0);
            currentTerminateErase();
            for (int i = 0; i < S.Length; i++)
            {
                double x = S[i].X;
                S[i].X = S[i].Y * -1;
                S[i].Y = x;
                if (((int)((S[i].Y + Position.Y) + 2)) >= Rows)
                {
                    move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Columns / 2) - 1)) < 0)
                {
                    move = false;
                }
                else if (((int)(S[i].X + Position.X) + ((Columns / 2) - 1)) >= Rows)
                {
                    move = false;
                }
                else if (BlockControls[((int)(S[i].X + Position.X) + ((Columns / 2) - 1)),
                    (int)(S[i].Y + Position.Y) + 2].Background != NoBrush)
                {
                    move = false;
                }
            }
            if (move)
            {
                currentTerminate.Rotate();
                currentTerminateDraw();
            }
            else
            {
                currentTerminateDraw();
            }
        }
    }
    public class Terminate
    {
        Point currentPosition;
        Point[] currentShape;
        Brush currentColor;
        bool rotate;
        public Terminate()
        {
            currentPosition = new Point(0, 0);
            currentColor = Brushes.Transparent;
            currentShape = setRandomShape();
        }
        public Brush getCurrentColor { get { return currentColor; } }
        public Point getCurrentPosition { get { return currentPosition; } }
        public Point[] getCurrentShape { get { return currentShape; } }
        public void moveLeft()
        {
            currentPosition.X -= 1;
        }
        public void moveRight()
        {
            currentPosition.X += 1;
        }
        public void moveDown()
        {
            currentPosition.Y += 1;
        }
        public void Rotate()
        {
            if (rotate)
            {
                for (int i = 0; i < currentShape.Length; i++)
                {
                    double x = currentShape[i].X;
                    currentShape[i].X = currentShape[i].Y * -1;
                    currentShape[i].Y = x;
                }
            }
        }
        Point[] setRandomShape()
        {
            Random random = new Random();
            switch (random.Next() % 7)
            {
                case 0: // I
                    rotate = true;
                    currentColor = Brushes.Cyan;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(-1,0),
                            new Point(1,0),
                            new Point(2,0)
                    };
                case 1: // J
                    rotate = true;
                    currentColor = Brushes.Blue;
                    return new Point[]
                    {
                            new Point(1,-1),
                            new Point(-1,0),
                            new Point(0,0),
                            new Point(1,0)
                    };
                case 2: // L
                    rotate = true;
                    currentColor = Brushes.Orange;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(-1,0),
                            new Point(1,0),
                            new Point(1,-1)
                    };
                case 3: // O
                    rotate = false;
                    currentColor = Brushes.Yellow;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(0,1),
                            new Point(1,0),
                            new Point(1,1)
                    };
                case 4: // S
                    rotate = true;
                    currentColor = Brushes.Green;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(-1,0),
                            new Point(0,-1),
                            new Point(1,0)
                    };
                case 5: // T
                    rotate = true;
                    currentColor = Brushes.Purple;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(-1,0),
                            new Point(0,-1),
                            new Point(1,0)
                    };
                case 6: // Z
                    rotate = true;
                    currentColor = Brushes.Red;
                    return new Point[]
                    {
                            new Point(0,0),
                            new Point(-1,0),
                            new Point(0,1),
                            new Point(1,1)
                    };
                default:
                    return null;
            }
        }
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer Timer;
        Board myBoard;
        public MainWindow()
        {
            InitializeComponent();
        }
        void MainWindow_Initilized(object sender, EventArgs e)
        {
            Timer = new DispatcherTimer();
            Timer.Tick += new EventHandler(GameTick);
            Timer.Interval = new TimeSpan(0, 0, 0, 0, 400);
            GameStart();
        }
        private void GameStart()
        {
            MainGrid.Children.Clear();
            myBoard = new Board(MainGrid);
            Timer.Start();
        }
        private void GameTick(object sender, EventArgs e)
        {
            Score.Content = myBoard.GetScore.ToString("00000000000");
            Lines.Content = myBoard.GetLines.ToString("00000000000");
            myBoard.CurrentTerminateMoveDown();
        }
        private void GamePause()
        {
            if (Timer.IsEnabled) Timer.Stop();
            else Timer.Start();
        }
        private void HandleKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    if (Timer.IsEnabled) myBoard.CurrentTerminateMoveLeft();
                    break;
                case Key.Right:
                    if (Timer.IsEnabled) myBoard.CurrentTerminateMoveRight();
                    break;
                case Key.Down:
                    if (Timer.IsEnabled) myBoard.CurrentTerminateMoveDown();
                    break;
                case Key.Up:
                    if (Timer.IsEnabled) myBoard.CurrentTerminateRotate();
                    break;
                case Key.F2:
                    GameStart();
                    break;
                case Key.F3:
                    GamePause();
                    break;
                default:
                    break;
            }
        }
    }
}
