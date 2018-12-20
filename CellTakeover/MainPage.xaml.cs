using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Color = System.Drawing.Color;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CellTakeover
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal const int NumberOfColumnsAndRows = 50;

        private readonly int[][] _cellValues = new int[NumberOfColumnsAndRows][];
        private Random _random = new Random();

        private AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Windows.UI.Colors.Brown
        };
        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly List<Player> _players = new List<Player>();

        public MainPage()
        {
            InitializeComponent();

            _players.Add(new Player
            {
                Name = "Player 1",
                Color = new SolidColorBrush(Colors.Blue)
            });

            _players.Add(new Player
            {
                Name = "Player 1",
                Color = new SolidColorBrush(Colors.Red)
            });

            for (int i = 0; i < NumberOfColumnsAndRows; i++)
            {
                _cellValues[i] = new int[100];
            }
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var uniformGrid = sender as UniformGrid;
            uniformGrid.Columns = NumberOfColumnsAndRows;
            uniformGrid.Rows = NumberOfColumnsAndRows;

            var numberOfCells = NumberOfColumnsAndRows * NumberOfColumnsAndRows;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);

            for (var i = 0; i < numberOfCells; i++)
            {
                uniformGrid.Children.Add(new Button
                {
                    Style = Resources["ButtonRevealStyle"] as Style,
                    Background = _emptyCellBrush,
                    BorderBrush = blackSolidColorBrush,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch
                });
            }

            var cellsPerPlayer = NumberOfColumnsAndRows * NumberOfColumnsAndRows / _players.Count;

            var rangeOfCellsPerPlayer = NumberOfColumnsAndRows * NumberOfColumnsAndRows / _players.Count -
                                        2 * NumberOfColumnsAndRows;

            for(int i = 0; i < _players.Count; i++)
            {
                var firstCandidateStartCell = cellsPerPlayer * i;
                //--make sure there is at least 2 rows between starting cells
                var endCandidateStartCell = firstCandidateStartCell + cellsPerPlayer - NumberOfColumnsAndRows * 2;
                var startCellIndex = _random.Next(firstCandidateStartCell, endCandidateStartCell);
                var element = MainGrid.Children[startCellIndex] as Button;
                element.Background = _players[i].Color;
            }
        }
    }
}
