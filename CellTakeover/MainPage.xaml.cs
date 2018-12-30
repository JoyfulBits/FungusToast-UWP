using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Logic;
using Microsoft.Toolkit.Uwp.UI.Animations;

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



        private readonly Dictionary<int, Grid> _playerNumberToPlayerGrid = new Dictionary<int, Grid>();
        private readonly Dictionary<int, List<Button>> _playerNumberToMutationButtons = new Dictionary<int, List<Button>>();
        private readonly Dictionary<int, TextBlock> _playerNumberToMutationPointAnnouncementTextBlock = new Dictionary<int, TextBlock>();
        private readonly Dictionary<int, ContentDialog> _playerNumberToSkillTreeDialog = new Dictionary<int, ContentDialog>();
        private readonly Dictionary<int, Button> _playerNumberToSkillTreeButton = new Dictionary<int, Button>();
        private readonly Dictionary<int, SolidColorBrush> _playerNumberToColorBrushDictionary = new Dictionary<int, SolidColorBrush>();

        private char _deadCellSymbol = '☠';

        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);
        private readonly Thickness _activeThickness = new Thickness(8);

        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);

        //--TODO introduce dependency injection framework
        private ICellGrowthCalculator _cellGrowthCalculator;
        private ICellRegrowthCalculator _cellRegrowthCalculator;
        private ISurroundingCellCalculator _surroundingCellCalculator;
        private GenerationAdvancer _generationAdvancer;


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

        private void PlayerGrid_Loaded(object sender, RoutedEventArgs e)
        {
            var playerStackPanel = sender as Grid;
            var playerNumber = int.Parse(playerStackPanel.Name);
            _playerNumberToPlayerGrid.Add(playerNumber, playerStackPanel);
        }

        private async void Grow_OnClick(object sender, RoutedEventArgs e)
        {
            GrowButton.IsEnabled = false;
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

            if (MutationConsumptionRound())
            {
                foreach (var player in ViewModel.Players)
                {
                    await IncreasePlayerMutationPoints(player);
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
                        IncreasePlayerMutationPoints(player);
                    }
                }

                //--since it's not a spending round we can keep the grow button enabled
                GrowButton.IsEnabled = true;
            }
        }

        private bool MutationConsumptionRound()
        {
            return ViewModel.GenerationNumber % ViewModel.NumberOfGenerationsBetweenFreeMutations == 0;
        }

        private async Task IncreasePlayerMutationPoints(IPlayer player)
        {
            player.AvailableMutationPoints++;
            var mutationPointAnnouncementMessage =
                _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerNumber];

            await mutationPointAnnouncementMessage.Fade(1, 300, 0, easingMode: EasingMode.EaseInOut).StartAsync();
            mutationPointAnnouncementMessage.Fade(0, 2000, 305).StartAsync();
        }

        private void PromptForMutationChoice()
        {
            foreach (var player in ViewModel.Players)
            {
                var playerGrid = _playerNumberToPlayerGrid[player.PlayerNumber];
                playerGrid.BorderBrush = _activeBorderBrush;
                playerGrid.BorderThickness = _activeThickness;
                var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerNumber];
                skillTreeButton.BorderBrush = _activeBorderBrush;
                skillTreeButton.BorderThickness = _activeThickness;
                EnablePlayerMutationButtons(player);
            }
        }

        private void CheckForRemainingMutationPoints(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
            {
                return;
            }

            DisablePlayerMutationButtons(player);

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

        private void EnablePlayerMutationButtons(IPlayer player)
        {
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerNumber];
            foreach (var mutationButton in playerMutationButtons)
            {
                mutationButton.IsEnabled = true;
            }
        }

        private void DisablePlayerMutationButtons(IPlayer player)
        {
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerNumber];

            foreach (var button in playerMutationButtons)
            {
                button.IsEnabled = false;
            }

            var playerGrid = _playerNumberToPlayerGrid[player.PlayerNumber];
            playerGrid.BorderBrush = _normalBorderBrush;
            playerGrid.BorderThickness = _normalThickness;

            var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerNumber];
            skillTreeButton.BorderBrush = _normalBorderBrush;
            skillTreeButton.BorderThickness = _normalThickness;
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
            if (player.AvailableMutationPoints > 0 && MutationConsumptionRound())
            {
                button.IsEnabled = true;
            }
            playerButtons.Add(button);
        }

        private void MutationPointMessage_Loaded(object sender, RoutedEventArgs e)
        {
            var mutationPointMessageTextBlock = sender as TextBlock;
            var player = mutationPointMessageTextBlock.DataContext as IPlayer;
            _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerNumber] = mutationPointMessageTextBlock;
        }

        private void AvailabledMutationTextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            ToolTip toolTip = new ToolTip
            {
                Content = $"Accumulated Mutation Points can be spent every {ViewModel.NumberOfGenerationsBetweenFreeMutations} generations to enhance your organism."
            };
            ToolTipService.SetToolTip(textBlock, toolTip);
        }

        private void SkillTreeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;
            _playerNumberToSkillTreeDialog[player.PlayerNumber] = skillTreeDialog;
        }

        private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var contentDialog = _playerNumberToSkillTreeDialog[player.PlayerNumber];
            await contentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void SkillTreeButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            _playerNumberToSkillTreeButton[player.PlayerNumber] = button;
        }
    }
}
