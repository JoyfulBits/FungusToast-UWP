using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Logic;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CellTakeover
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly Dictionary<int, BioCell> _currentLiveCells = new Dictionary<int, BioCell>();
        private readonly Dictionary<int, Player> _nextLiveCells = new Dictionary<int, Player>();
        //private readonly HashSet<int> _deadCells = new HashSet<int>();

        private readonly Random _random = new Random();

        private AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Windows.UI.Colors.Brown
        };
        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly List<Player> _players = new List<Player>();

        //--TODO introduce dependency injection framework
        private CellGrowthCalculator _cellGrowthCalculator = new CellGrowthCalculator();

        public MainPage()
        {
            InitializeComponent();

            _players.Add(new Player("Player 1", Colors.Blue, 1, _cellGrowthCalculator));
            _players.Add(new Player("Player 2", Colors.Red, 1, _cellGrowthCalculator));
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var uniformGrid = sender as UniformGrid;
            uniformGrid.Columns = GameSettings.NumberOfColumnsAndRows;
            uniformGrid.Rows = GameSettings.NumberOfColumnsAndRows;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);

            for (var i = 0; i < GameSettings.NumberOfCells; i++)
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

            var cellsPerPlayer = GameSettings.NumberOfCells / _players.Count;

            for(int i = 0; i < _players.Count; i++)
            {
                var player = _players[i];
                var firstCandidateStartCell = cellsPerPlayer * i;
                //--make sure there is at least 2 rows between starting cells
                var endCandidateStartCell = firstCandidateStartCell + cellsPerPlayer - GameSettings.NumberOfColumnsAndRows * 2;
                var startCellIndex = _random.Next(firstCandidateStartCell, endCandidateStartCell);
                var element = MainGrid.Children[startCellIndex] as Button;
                element.Background = new SolidColorBrush(_players[i].Color);
                _currentLiveCells.Add(startCellIndex, player.MakeCell(startCellIndex));
            }
        }

        private void Grow_OnClick(object sender, RoutedEventArgs e)
        {
            for (var i = 0; i < GameSettings.NumberOfCells; i++)
            {
                if (_currentLiveCells.ContainsKey(i))
                {
                    Grow(_currentLiveCells[i]);
                }
            }
        }

        private async void Grow(BioCell cell)
        {
            var newCells = cell.RunCellGrowth(_currentLiveCells);
            foreach (var newCell in newCells)
            {
                var element = MainGrid.Children[newCell.CellIndex] as Button;
                element.Background = new SolidColorBrush(newCell.CellColor);
                _currentLiveCells.Add(newCell.CellIndex, newCell);
            }
        }
    }
}
