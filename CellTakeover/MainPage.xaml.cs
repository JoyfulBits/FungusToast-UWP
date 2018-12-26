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
            TintColor = Windows.UI.Colors.Brown
        };

        private char _deadCellSymbol = '☠';
        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);
        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);

        private readonly Dictionary<int, StackPanel> _playerNumberToPlayerStackPanel = new Dictionary<int, StackPanel>();
        private readonly Dictionary<int, List<Button>> _playerNumberToMutationButtons = new Dictionary<int, List<Button>>();


        private readonly Thickness _activeThickness = new Thickness(10);
        private SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);

        public const int NumberOfGenerationsBetweenFreeMutations = 5;

        //--TODO introduce dependency injection framework
        private readonly ICellGrowthCalculator _cellGrowthCalculator = new CellGrowthCalculator();
        private readonly ISurroundingCellCalculator _surroundingCellCalculator = new SurroundingCellCalculator(GameSettings.NumberOfColumnsAndRows);
        private readonly GenerationAdvancer _generationAdvancer = new GenerationAdvancer();
        private readonly Dictionary<int, SolidColorBrush> _playerNumberToColorBrushDictionary;
        private readonly IMutationOptionGenerator _mutationOptionGenerator = new MutationOptionGenerator();


        public MainPage()
        {
            InitializeComponent();
            ViewModel = new CellTakeoverViewModel();
            var players = new List<IPlayer>();
            players.Add(new Player("Player 1", Colors.Blue, 1, "☣", _cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 2", Colors.Red, 2, "☢", _cellGrowthCalculator, _surroundingCellCalculator));
            players.Add(new Player("Player 3", Colors.DarkMagenta, 3, "⚠", _cellGrowthCalculator, _surroundingCellCalculator));
            ViewModel.Players = players;

            _playerNumberToColorBrushDictionary = new Dictionary<int, SolidColorBrush>();
            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerNumber, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerNumber, new List<Button>());
            }
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
                ViewModel.AddNewLiveCell(startCellIndex, player.MakeCell(startCellIndex));
            }
        }

        private void AddNewCells(NextGenerationResults nextGenerationResult)
        {
            foreach (var newCell in nextGenerationResult.NewLiveCells)
            {
                //--its possible for two different cells to split to the same cell. For now, the first cell wins
                if (!ViewModel.CurrentLiveCells.ContainsKey(newCell.CellIndex))
                {
                    var element = MainGrid.Children[newCell.CellIndex] as Button;
                    element.Background = _playerNumberToColorBrushDictionary[newCell.Player.PlayerNumber];
                    element.Content = newCell.Player.CharacterSymbol;
                    ViewModel.AddNewLiveCell(newCell.CellIndex, newCell);
                }
            }
        }

        private void KillCells(NextGenerationResults nextGenerationResult)
        {
            foreach (var newDeadCell in nextGenerationResult.NewDeadCells)
            {
                if (!ViewModel.CurrentDeadCells.ContainsKey(newDeadCell.CellIndex))
                {
                    var element = MainGrid.Children[newDeadCell.CellIndex] as Button;
                    element.Background = _deadCellBrush;
                    element.Content = _deadCellSymbol;
                    ViewModel.AddNewDeadCell(newDeadCell.CellIndex, newDeadCell);
                }

                if (ViewModel.CurrentLiveCells.ContainsKey(newDeadCell.CellIndex))
                {
                    ViewModel.RemoveLiveCell(newDeadCell.CellIndex);
                }
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
            AddNewCells(nextGenerationResult);

            KillCells(nextGenerationResult);

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
                foreach (var player in players)
                {
                    var mutationOption = _mutationOptionGenerator.GetMutationChoices(player, ViewModel.Players);
                    var playerMutationChoice = new Tuple<IPlayer, MutationChoice>(player, mutationOption);
                    ViewModel.MutationChoices.Push(playerMutationChoice);
                }

                PromptForMutationChoice();
            }
            else
            {
                //--players only get bonus mutations if not on a round with a free mutation
                foreach (var player in players)
                {
                    if (player.GetsFreeMutation())
                    {
                        var mutationOption = _mutationOptionGenerator.GetMutationChoices(player, ViewModel.Players);
                        var playerMutationChoice = new Tuple<IPlayer, MutationChoice>(player, mutationOption);
                        ViewModel.MutationChoices.Push(playerMutationChoice);
                        PromptForMutationChoice();
                    }
                }
            }
        }

        private bool PromptForMutationChoice()
        {
            if (ViewModel.MutationChoices.Count > 0)
            {
                var mutationChoiceTuple = ViewModel.MutationChoices.Pop();
                GrowButton.IsEnabled = false;
                var playerNumber = mutationChoiceTuple.Item1.PlayerNumber;
                var playerStackPanel = _playerNumberToPlayerStackPanel[playerNumber];
                playerStackPanel.BorderBrush = _activeBorderBrush;
                playerStackPanel.BorderThickness = _activeThickness;
                var mutationChoice = mutationChoiceTuple.Item2;
                var playerMutationButtons = _playerNumberToMutationButtons[playerNumber];

                if (mutationChoice.IncreaseMutationChance)
                {
                    var mutationChanceButton = playerMutationButtons.First(x => x.Name == "MutationChanceButton");
                    mutationChanceButton.IsEnabled = true;
                }

                if (mutationChoice.IncreaseCornerGrowthChance)
                {
                    var cornerGrowthButton = playerMutationButtons.First(x => x.Name == "CornerGrowthButton");
                    cornerGrowthButton.IsEnabled = true;
                }

                if (mutationChoice.DecreaseHealthyCellDeathChance)
                {
                    var reduceHealthyCellDeathChanceButton = playerMutationButtons.First(x => x.Name == "HealthyCellDeathChanceButton");
                    reduceHealthyCellDeathChanceButton.IsEnabled = true;
                }
            }
            else
            {
                GrowButton.IsEnabled = true;
            }

            return ViewModel.MutationChoices.Count > 0;
        }

        private void IncreaseMutationChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as Player;
            player.IncreaseMutationChance();

            DeActivatePlayerMutationButtons(player);

            PromptForMutationChoice();
        }

        private void ReduceHealthyCellDeathChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as Player;
            player.DecreaseHealthyCellDeathChance();

            DeActivatePlayerMutationButtons(player);

            PromptForMutationChoice();
        }

        private void IncreaseCornerGrowthChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as Player;
            player.IncreaseCornerGrowth();

            DeActivatePlayerMutationButtons(player);

            PromptForMutationChoice();
        }

        private void DeActivatePlayerMutationButtons(Player player)
        {
            GrowButton.IsEnabled = false;
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
