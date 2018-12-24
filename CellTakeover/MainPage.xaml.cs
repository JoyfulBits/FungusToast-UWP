using System;
using System.Collections.Generic;
using System.Threading;
using Windows.UI;
using Windows.UI.Text;
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
        public CellTakeoverViewModel ViewModel { get; set; }

        private readonly Random _random = new Random();

        private AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Windows.UI.Colors.Brown
        };
        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);


        //--TODO introduce dependency injection framework
        private readonly ICellGrowthCalculator _cellGrowthCalculator = new CellGrowthCalculator();
        private readonly ISurroundingCellCalculator _surroundingCellCalculator = new SurroundingCellCalculator(GameSettings.NumberOfColumnsAndRows);
        private readonly GenerationAdvancer _generationAdvancer = new GenerationAdvancer();

        public MainPage()
        {
            InitializeComponent();
            ViewModel = new CellTakeoverViewModel();
            var players = new List<Player>();
            players.Add(new Player("Player 1", Colors.Blue, 1, "A", _cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 2", Colors.Red, 2, "B",_cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 3", Colors.DarkMagenta, 3, "C", _cellGrowthCalculator, _surroundingCellCalculator));
            ViewModel.Players = players;
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var uniformGrid = sender as UniformGrid;
            uniformGrid.Columns = GameSettings.NumberOfColumnsAndRows;
            uniformGrid.Rows = GameSettings.NumberOfColumnsAndRows;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            Thickness noPaddingOrMargin = new Thickness(0);

            for (var i = 0; i < GameSettings.NumberOfCells; i++)
            {
                uniformGrid.Children.Add(new Button
                {
                    Style = Resources["ButtonRevealStyle"] as Style,
                    Background = _emptyCellBrush,
                    BorderBrush = blackSolidColorBrush,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    FontStretch = FontStretch.UltraCondensed,
                    Margin = noPaddingOrMargin,
                    Padding = noPaddingOrMargin,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                });
            }

            var cellsPerPlayer = GameSettings.NumberOfCells / ViewModel.Players.Count;

            for(int i = 0; i < ViewModel.Players.Count; i++)
            {
                var player = ViewModel.Players[i];
                var firstCandidateStartCell = cellsPerPlayer * i;
                //--make sure there is at least 2 rows between starting cells
                var endCandidateStartCell = firstCandidateStartCell + cellsPerPlayer - GameSettings.NumberOfColumnsAndRows * 2;
                var startCellIndex = _random.Next(firstCandidateStartCell, endCandidateStartCell);
                var element = MainGrid.Children[startCellIndex] as Button;
                element.Background = new SolidColorBrush(ViewModel.Players[i].Color);
                element.Content = player.CharacterSymbol;
                ViewModel.CurrentLiveCells.Add(startCellIndex, player.MakeCell(startCellIndex));
            }
        }

        private void Grow_OnClick(object sender, RoutedEventArgs e)
        {
            var newCells = _generationAdvancer.NextGeneration(ViewModel.CurrentLiveCells);
            foreach (var newCell in newCells)
            {
                //--its possible for two different cells to split to the same cell. For now, the first cell wins
                if (!ViewModel.CurrentLiveCells.ContainsKey(newCell.CellIndex))
                {
                    var element = MainGrid.Children[newCell.CellIndex] as Button;
                    element.Background = new SolidColorBrush(newCell.CellColor);
                    element.Content = newCell.Player.CharacterSymbol;
                    ViewModel.CurrentLiveCells.Add(newCell.CellIndex, newCell);
                }
            }

            ViewModel.GenerationNumber++;
        }
    }
}
