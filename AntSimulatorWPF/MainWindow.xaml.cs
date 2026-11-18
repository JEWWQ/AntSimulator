using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AntSimulatorWPF
{
    public partial class MainWindow : Window
    {
        private int fieldSize = 20;
        private int cellSize = 20;

        private int[,] field;
        private Rectangle[,] fieldRects;

        private Polygon antVisual;

        private int antX = 0;
        private int antY = 0;
        private int direction = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            fieldSize = int.Parse(((ComboBoxItem)cbSize.SelectedItem).Content.ToString());

            GameCanvas.Children.Clear();
            GameCanvas.Width = fieldSize * cellSize;
            GameCanvas.Height = fieldSize * cellSize;

            field = new int[fieldSize, fieldSize];
            fieldRects = new Rectangle[fieldSize, fieldSize];

            Random rnd = new Random();

            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    field[x, y] = rnd.Next(100) < 30 ? 1 : 0;

                    Rectangle rect = new Rectangle
                    {
                        Width = cellSize,
                        Height = cellSize,
                        Fill = field[x, y] == 1 ? Brushes.Black : Brushes.White,
                        Stroke = Brushes.LightGray,
                        StrokeThickness = 0.5
                    };

                    Canvas.SetLeft(rect, x * cellSize);
                    Canvas.SetTop(rect, y * cellSize);

                    GameCanvas.Children.Add(rect);
                    fieldRects[x, y] = rect;
                }
            }

            PointCollection points = new PointCollection
            {
                new Point(cellSize / 2.0, 2),
                new Point(2, cellSize - 2),
                new Point(cellSize - 2, cellSize - 2)
            };

            antVisual = new Polygon
            {
                Points = points,
                Fill = Brushes.Red,
                RenderTransformOrigin = new Point(0.5, 0.5)
            };

            antX = rnd.Next(0, fieldSize);
            antY = rnd.Next(0, fieldSize);
            direction = 0;

            antVisual.RenderTransform = new RotateTransform(0);

            GameCanvas.Children.Add(antVisual);
            UpdateAntPosition();

            txtStatus.Text = $"Поле готово. Муравей появился в [{antX}, {antY}].";
        }

        private void Action_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                DragDrop.DoDragDrop(btn, btn.Tag.ToString(), DragDropEffects.Copy);
            }
        }

        private void GameCanvas_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.Text) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private void GameCanvas_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                string action = e.Data.GetData(DataFormats.Text).ToString();
                DoAction(action);
            }
        }

        private void DoAction(string action)
        {
            if (antVisual == null) return;

            if (action == "Go")
            {
                if (direction == 0) antY--;
                else if (direction == 1) antX++;
                else if (direction == 2) antY++;
                else if (direction == 3) antX--;

                if (antX < 0) antX = fieldSize - 1;
                if (antX >= fieldSize) antX = 0;
                if (antY < 0) antY = fieldSize - 1;
                if (antY >= fieldSize) antY = 0;

                UpdateAntPosition();
                txtStatus.Text = $"Муравей шагнул. Теперь он в [{antX}, {antY}].";
            }
            else if (action == "Turn")
            {
                direction = (direction + 1) % 4;
                RotateAnt();
                txtStatus.Text = "Муравей повернул.";
            }
            else if (action == "Paint")
            {
                if (field[antX, antY] == 0)
                {
                    field[antX, antY] = 1;
                    fieldRects[antX, antY].Fill = Brushes.Black;
                    txtStatus.Text = "Клетка закрашена.";
                }
                else
                {
                    txtStatus.Text = "Клетка уже черная.";
                }
            }
        }

        private void UpdateAntPosition()
        {
            Canvas.SetLeft(antVisual, antX * cellSize);
            Canvas.SetTop(antVisual, antY * cellSize);
        }

        private void RotateAnt()
        {
            double angle = direction * 90;

            if (antVisual.RenderTransform == null || !(antVisual.RenderTransform is RotateTransform))
            {
                antVisual.RenderTransform = new RotateTransform();
            }

            RotateTransform rotate = (RotateTransform)antVisual.RenderTransform;
            rotate.Angle = angle;
        }
    }
}