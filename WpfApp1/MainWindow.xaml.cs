
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            // Get the number of rows and columns from the sliders
            int numRows = Convert.ToInt32(sliderRows.Value);
            int numColumns = Convert.ToInt32(sliderColumns.Value);

            // Set the number of rows and columns in the grid
            myGrid.RowDefinitions.Clear();
            myGrid.ColumnDefinitions.Clear();
            for (int i = 0; i < numRows; i++)
            {
                myGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < numColumns; i++)
            {
                myGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            // Clear the existing elements in the grid
            myGrid.Children.Clear();


            // Create a 2D array to keep track of occupied cells
            bool[,] occupiedCells = new bool[numRows, numColumns];

            // Create the elements
            Random random = new Random();
            for (int i = 0; i < ((numColumns + numRows) * 0.5); i++)
            {
                TextBlock txt = new();
                txt.Text = "✈"+i.ToString();
                txt.Foreground = new SolidColorBrush(Colors.White);
                txt.Background = new SolidColorBrush(Colors.Green);
                txt.Width = 16;
                txt.Height = 17;
                txt.Margin = new Thickness(5);

                int row, column;
                do
                {
                    row = random.Next(numRows);
                    column = random.Next(numColumns);
                } while (occupiedCells[row, column]);

                occupiedCells[row, column] = true;

                myGrid.Children.Add(txt);
                Grid.SetColumn(txt, column);
                Grid.SetRow(txt, row);
            }

            for (int i = 0; i <= ((numColumns + numRows) * 0.1); i++)
            {
                TextBlock txt = new();
                txt.Text = "🏢🏢";
                txt.Foreground = new SolidColorBrush(Colors.White);
                txt.Background = new SolidColorBrush(Colors.Red);
                txt.Width = 33;
                txt.Height = 18;
                txt.Margin = new Thickness(5);

               

               int row = random.Next(numRows);
               int column = random.Next(numColumns);

                myGrid.Children.Add(txt);
                Grid.SetColumn(txt, column);
                Grid.SetRow(txt, row);
            }

        }


        private DispatcherTimer timer;

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Tick += MoveTextBlocks;
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
        }

        private void MoveTextBlocks(object sender, EventArgs e)
        {
            int numRows = Convert.ToInt32(sliderRows.Value);
            int numColumns = Convert.ToInt32(sliderColumns.Value);

            List<string> log = new List<string>();

            for (int i = myGrid.Children.Count - 1; i >= 0; i--)
            {
                UIElement element = myGrid.Children[i];
                TextBlock txt = element as TextBlock;
                if (txt != null && txt.Background is SolidColorBrush brush && brush.Color == Colors.Green)
                {
                    int row = Grid.GetRow(txt);
                    int column = Grid.GetColumn(txt);

                    bool isTouchingRed = false;
                    Random random = new Random();
                    while (true)
                    {
                        int r = random.Next(-1, 2);
                        int c = random.Next(-1, 2);
                        if (r == 0 && c == 0)
                        {
                            continue;
                        }

                        int newRow = row + r;
                        int newColumn = column + c;
                        if (newRow >= 0 && newRow < numRows && newColumn >= 0 && newColumn < numColumns)
                        {
                            bool isOccupied = false;
                            foreach (UIElement child in myGrid.Children)
                            {
                                if (child != txt && Grid.GetRow(child) == newRow && Grid.GetColumn(child) == newColumn)
                                {
                                    TextBlock childTxt = child as TextBlock;
                                    if (childTxt != null && childTxt.Background is SolidColorBrush childBrush && childBrush.Color == Colors.Red)
                                    {
                                        isTouchingRed = true;
                                        break;
                                    }
                                    else
                                    {
                                        isOccupied = true;
                                        break;
                                    }
                                }
                            }

                            if (!isOccupied)
                            {
                                Grid.SetColumn(txt, newColumn);
                                Grid.SetRow(txt, newRow);
                                log.Add($"Moved {txt.Text} from ({row}, {column}) to ({newRow}, {newColumn})");
                                break;
                            }
                        }
                    }

                    if (isTouchingRed)
                    {
                        myGrid.Children.Remove(txt);
                        for (int j = myGrid.Children.Count - 1; j >= 0; j--)
                        {
                            UIElement child = myGrid.Children[j];
                            TextBlock childTxt = child as TextBlock;
                            if (childTxt != null && childTxt.Background is SolidColorBrush childBrush && childBrush.Color == Colors.Red && Grid.GetRow(childTxt) == row && Grid.GetColumn(childTxt) == column)
                            {
                                myGrid.Children.Remove(child);
                                log.Add($"Removed {childTxt.Text} at ({row}, {column})");
                            }
                        }
                    }
                }
            }

            // Bind the log List to the ListBox
            listBoxLog.ItemsSource = log;
        }
    }
}





