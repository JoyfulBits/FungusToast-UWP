using System;
using System.Collections.Generic;
using System.Linq;
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

        private readonly AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Colors.White
        };

        private char _deadCellSymbol = '☠';
        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);
        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);

        private readonly Dictionary<int, StackPanel> _playerNumberToPlayerStackPanel = new Dictionary<int, StackPanel>();
        private readonly Dictionary<int, List<Button>> _playerNumberToMutationButtons = new Dictionary<int, List<Button>>();


        private readonly Thickness _activeThickness = new Thickness(10);
        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);
        private readonly Dictionary<int, SolidColorBrush> _playerNumberToColorBrushDictionary = new Dictionary<int, SolidColorBrush>();


        public const int NumberOfGenerationsBetweenFreeMutations = 5;

        //--TODO introduce dependency injection framework
        private ICellGrowthCalculator _cellGrowthCalculator;
        private ICellRegrowthCalculator _cellRegrowthCalculator;
        private ISurroundingCellCalculator _surroundingCellCalculator;
        private GenerationAdvancer _generationAdvancer;
        private IMutationOptionGenerator _mutationOptionGenerator;


        public MainPage()
        {
            InitializeDependencies();
            InitializeComponent();
            ViewModel = new CellTakeoverViewModel();
            var players = new List<IPlayer>();
            players.Add(new Player("Player 1", Colors.SkyBlue, 1, "☣", _cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 2", Colors.Blue, 2, "☢", _cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 3", Colors.PaleVioletRed, 3, "⚠", _cellGrowthCalculator, _surroundingCellCalculator));
            ViewModel.Players = players;

            _playerNumberToColorBrushDictionary = new Dictionary<int, SolidColorBrush>();
            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerNumber, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerNumber, new List<Button>());
            }
        }

        private void InitializeDependencies()
        {
            _cellGrowthCalculator = new CellGrowthCalculator();
            _surroundingCellCalculator = new SurroundingCellCalculator(GameSettings.NumberOfColumnsAndRows);
            _cellRegrowthCalculator = new CellRegrowthCalculator(_surroundingCellCalculator);
            _generationAdvancer = new GenerationAdvancer(_cellRegrowthCalculator);
            _mutationOptionGenerator = new MutationOptionGenerator();
        }


        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var uniformGrid = sender as UniformGrid;
            uniformGrid.Columns = GameSettings.NumberOfColumnsAndRows;
            uniformGrid.Rows = GameSettings.NumberOfColumnsAndRows;
            //--make the grid a square since it wasn't doing that for some reason
            uniformGrid.Width = uniformGrid.ActualHeight;

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
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 10
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
                var button = MainGrid.Children[startCellIndex] as Button;
                button.Background = new SolidColorBrush(ViewModel.Players[i].Color);
                button.Content = "  ";
                //button.Content = player.PlayerSymbol;
                ViewModel.AddNewLiveCell(player.MakeCell(startCellIndex));
            }
        }

        private void AddNewCells(List<BioCell> newLiveCells)
        {
            foreach (var newCell in newLiveCells)
            {
                //--its possible for two different cells to split to the same cell. For now, the first cell wins
                if (!ViewModel.CurrentLiveCells.ContainsKey(newCell.CellIndex))
                {
                    var button = MainGrid.Children[newCell.CellIndex] as Button;
                    button.Background = _playerNumberToColorBrushDictionary[newCell.Player.PlayerNumber];
                    //button.Content = newCell.Player.PlayerSymbol;
                    ViewModel.AddNewLiveCell(newCell);
                }
            }
        }

        private void KillCells(List<BioCell> newDeadCells)
        {
            foreach (var newDeadCell in newDeadCells)
            {
                if (!ViewModel.CurrentDeadCells.ContainsKey(newDeadCell.CellIndex))
                {
                    var button = MainGrid.Children[newDeadCell.CellIndex] as Button;
                    button.Background = _deadCellBrush;
                    button.Content = _deadCellSymbol;
                    ViewModel.AddNewDeadCell(newDeadCell);
                }

                ViewModel.RemoveLiveCell(newDeadCell.CellIndex);
            }
        }

        private void RegrowCells(List<BioCell> regrownCells)
        {
            foreach (var regrownCell in regrownCells)
            {
                ViewModel.RegrowCell(regrownCell);

                var element = MainGrid.Children[regrownCell.CellIndex] as Button;
                element.Background = _playerNumberToColorBrushDictionary[regrownCell.Player.PlayerNumber];
                element.Content = string.Empty;
            }
        }

        private void PlayerStackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            var playerStackPanel = sender as StackPanel;
            var playerNumber = int.Parse(playerStackPanel.Name);
            _playerNumberToPlayerStackPanel.Add(playerNumber, playerStackPanel);
        }

        private void Grow_OnClick(object sender, RoutedEventArgs e)
        {
            var nextGenerationResult = _generationAdvancer.NextGeneration(ViewModel.CurrentLiveCells, ViewModel.CurrentDeadCells);
            AddNewCells(nextGenerationResult.NewLiveCells);

            KillCells(nextGenerationResult.NewDeadCells);

            RegrowCells(nextGenerationResult.RegrownCells);

            ViewModel.GenerationNumber++;

            List<IPlayer> players;
            //--every other round the players are checked in reverse order for fairness
            if (ViewModel.GenerationNumber % 2 == 0)
            {
                players = new List<IPlayer>(ViewModel.Players);
                players.Reverse();
            }
            else
            {
                players = ViewModel.Players;
            }

            if (ViewModel.GenerationNumber % NumberOfGenerationsBetweenFreeMutations == 0)
            {
                foreach (var player in ViewModel.Players)
                {
                    player.AvailableMutationPoints++;
                }

                //--only allow points to be spent every X rounds
                PromptForMutationChoice();
            }
            else
            {
                //--players only get bonus mutations if not on a round with a free mutation
                foreach (var player in players)
                {
                    if (player.GetsFreeMutation())
                    {
                        player.AvailableMutationPoints++;
                    }
                }
            }
        }

        private void PromptForMutationChoice()
        {
            GrowButton.IsEnabled = false;

            foreach (var player in ViewModel.Players)
            {
                var playerStackPanel = _playerNumberToPlayerStackPanel[player.PlayerNumber];
                playerStackPanel.BorderBrush = _activeBorderBrush;
                playerStackPanel.BorderThickness = _activeThickness;
                var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerNumber];

                var mutationChanceButton = playerMutationButtons.First(x => x.Name == "MutationChanceButton");
                mutationChanceButton.IsEnabled = true;

                var cornerGrowthButton = playerMutationButtons.First(x => x.Name == "CornerGrowthButton");
                cornerGrowthButton.IsEnabled = true;

                var reduceHealthyCellDeathChanceButton = playerMutationButtons.First(x => x.Name == "HealthyCellDeathChanceButton");
                reduceHealthyCellDeathChanceButton.IsEnabled = true;

                var regrowthButton = playerMutationButtons.First(x => x.Name == "RegrowthButton");
                regrowthButton.IsEnabled = true;
            }
        }

        private void CheckForRemainingMutationPoints(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
            {
                return;
            }

            DeActivatePlayerMutationButtons(player);

            foreach (var p in ViewModel.Players)
            {
                if (p.AvailableMutationPoints > 0)
                {
                    
                    return;
                }
            }

            //--if no players have remaining mutation points then we can go back to growing
            GrowButton.IsEnabled = true;
        }

        private void IncreaseMutationChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseMutationChance();

            CheckForRemainingMutationPoints(player);
        }

        private void ReduceHealthyCellDeathChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.DecreaseHealthyCellDeathChance();

            CheckForRemainingMutationPoints(player);
        }

        private void IncreaseCornerGrowthChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseCornerGrowth();

            CheckForRemainingMutationPoints(player);
        }

        private void IncreaseRegrowthChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseRegrowthChance();

            CheckForRemainingMutationPoints(player);
        }

        private void DeActivatePlayerMutationButtons(IPlayer player)
        {
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerNumber];

            foreach (var button in playerMutationButtons)
            {
                button.IsEnabled = false;
            }

            var playerStackPanel = _playerNumberToPlayerStackPanel[player.PlayerNumber];
            playerStackPanel.BorderBrush = _normalBorderBrush;
            playerStackPanel.BorderThickness = _normalThickness;
        }

        public T FindElementByName<T>(FrameworkElement parentElement, string childName) where T : FrameworkElement
        {
            T childElement = null;
            var numberOfChildren = VisualTreeHelper.GetChildrenCount(parentElement);
            for (int i = 0; i < numberOfChildren; i++)
            {
                var child = VisualTreeHelper.GetChild(parentElement, i) as FrameworkElement;

                if (child == null)
                    continue;

                if (child is T && child.Name.Equals(childName))
                {
                    childElement = (T)child;
                    break;
                }

                childElement = FindElementByName<T>(child, childName);

                if (childElement != null)
                    break;
            }

            return childElement;
        }

        private void MutationOptionButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var playerButtons = _playerNumberToMutationButtons[player.PlayerNumber];
            playerButtons.Add(button);
        }
    }
}
