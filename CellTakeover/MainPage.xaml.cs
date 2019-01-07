using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
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

        private readonly AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Colors.White
        };

        private readonly Dictionary<int, Dictionary<string, Button>> _playerNumberToMutationButtons = new Dictionary<int, Dictionary<string, Button>>();
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
        }

        private void InitializeDependencies()
        {
            _cellGrowthCalculator = new CellGrowthCalculator();
            _surroundingCellCalculator = new SurroundingCellCalculator(GameSettings.NumberOfColumnsAndRows);
            _cellRegrowthCalculator = new CellRegrowthCalculator(_surroundingCellCalculator);
            _generationAdvancer = new GenerationAdvancer(_cellRegrowthCalculator);
        }


        private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            await GameSettingsDialog.ShowAsync();
        }

        private readonly List<Color> _availableColors = new List<Color>
        {
            Colors.SkyBlue,
            Colors.PaleVioletRed,
            Colors.Blue,
            Colors.Orange,
            Colors.Gray
        };

        private void GameStart_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var players = new List<IPlayer>();
            var numberOfHumanPlayers = int.Parse(NumberOfHumanPlayersComboBox.SelectedValue.ToString());
            for (int i = 0; i < numberOfHumanPlayers; i++)
            {
                players.Add(new Player($"Player {i}", _availableColors[i], i, _cellGrowthCalculator, _surroundingCellCalculator));
            }

            //TODO add AI players
            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerNumber, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerNumber, new Dictionary<string, Button>());
            }

            InitializePetriDishWithPlayerCells();
        }

        private void InitializePetriDishWithPlayerCells()
        {
            PetriDish.Columns = GameSettings.NumberOfColumnsAndRows;
            PetriDish.Rows = GameSettings.NumberOfColumnsAndRows;
            //--make the grid a square since it wasn't doing that for some reason
            PetriDish.Width = PetriDish.ActualHeight;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            Thickness noPaddingOrMargin = new Thickness(0);

            for (var i = 0; i < GameSettings.NumberOfCells; i++)
            {
                PetriDish.Children.Add(new Button
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
            for (int i = 0; i < ViewModel.Players.Count; i++)
            {
                var player = ViewModel.Players[i];
                player.LiveCells++;
                var firstCandidateStartCell = cellsPerPlayer * i;
                //--make sure there is at least 2 rows between starting cells
                var endCandidateStartCell = firstCandidateStartCell + cellsPerPlayer - GameSettings.NumberOfColumnsAndRows * 2;
                var startCellIndex = RandomNumberGenerator.Random.Next(firstCandidateStartCell, endCandidateStartCell);
                var button = PetriDish.Children[startCellIndex] as Button;
                button.Background = new SolidColorBrush(ViewModel.Players[i].Color);
                button.Content = "  ";
                //button.Content = player.PlayerSymbol;
                ViewModel.AddNewLiveCell(player.MakeCell(startCellIndex));
            }
        }

        private async void NextGenerationCycle()
        {
            for (int i = 0; i < ViewModel.NumberOfGenerationsBetweenFreeMutations; i++)
            {
                await NextGeneration();
            }

            foreach (var player in ViewModel.Players)
            {
                await IncreasePlayerMutationPoints(player);
            }

            //--only allow points to be spent every X rounds
            PromptForMutationChoice();
        }

        private async Task NextGeneration()
        {
            GrowButton.IsEnabled = false;

            List<IPlayer> players = ViewModel.Players;

            var nextGenerationResult = await Task.Run(() => _generationAdvancer.NextGeneration(ViewModel.CurrentLiveCells, ViewModel.CurrentDeadCells,
                players));

            AddNewCells(nextGenerationResult.NewLiveCells);

            KillCells(nextGenerationResult.NewDeadCells);

            RegrowCells(nextGenerationResult.RegrownCells);

            ViewModel.GenerationNumber++;

            foreach (var player in players)
            {
                var playerGrowthSummary = nextGenerationResult.PlayerGrowthSummaries[player.PlayerNumber];

                player.LiveCells += playerGrowthSummary.NewLiveCellCount;
                player.LiveCells += nextGenerationResult.PlayerNumberToNumberOfRegrownCells[player.PlayerNumber];
                player.LiveCells -= playerGrowthSummary.NewDeadCellCount;

                player.DeadCells += playerGrowthSummary.NewDeadCellCount;
                player.DeadCells -= nextGenerationResult.PlayerNumberToNumberOfDeadCellsEliminated[player.PlayerNumber];

                var numberOfRegrownCells = nextGenerationResult.PlayerNumberToNumberOfRegrownCells[player.PlayerNumber];
                player.RegrownCells += numberOfRegrownCells;

                if (player.GetsFreeMutation())
                {
                    await IncreasePlayerMutationPoints(player);
                }
            }

            //--since it's not a spending round we can keep the grow button enabled
            GrowButton.IsEnabled = true;

            await CheckForGameEnd();
        }

        private void AddNewCells(List<BioCell> newLiveCells)
        {
            foreach (var newCell in newLiveCells)
            {
                //--its possible for two different cells to split to the same cell. For now, the first cell wins
                if (!ViewModel.CurrentLiveCells.ContainsKey(newCell.CellIndex))
                {
                    var button = PetriDish.Children[newCell.CellIndex] as Button;
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
                    var button = PetriDish.Children[newDeadCell.CellIndex] as Button;
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

                var element = PetriDish.Children[regrownCell.CellIndex] as Button;
                element.Background = _playerNumberToColorBrushDictionary[regrownCell.Player.PlayerNumber];
                element.Content = string.Empty;
            }
        }

        private async void Grow_OnClick(object sender, RoutedEventArgs e)
        {
            NextGenerationCycle();
        }

        private async Task CheckForGameEnd()
        {
            if (ViewModel.TotalEmptyCells == 0)
            {
                if (ViewModel.GameEndCountDown == 0)
                {
                    ViewModel.TriggerGameOverResultOnPropertyChanged();
                    await GameEndContentDialog.ShowAsync();
                }
                else
                {
                    ViewModel.GameEndCountDown--;
                }
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
            if (player.GrowthScorecard.HealthyCellDeathChancePercentage <= 0)
            {
                button.IsEnabled = false;
            }

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
                if (mutationButton.Key == "ReduceHealthyCellDeathChanceButton" && player.GrowthScorecard.HealthyCellDeathChancePercentage <= 0)
                {
                    mutationButton.Value.IsEnabled = false;
                }
                else
                {
                    mutationButton.Value.IsEnabled = true;
                }
            }
        }

        private void DisablePlayerMutationButtons(IPlayer player)
        {
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerNumber];

            foreach (var button in playerMutationButtons)
            {
                button.Value.IsEnabled = false;
            }

            var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerNumber];
            skillTreeButton.BorderBrush = _normalBorderBrush;
            skillTreeButton.BorderThickness = _normalThickness;

            var dialog = _playerNumberToSkillTreeDialog[player.PlayerNumber];
            dialog.Hide();
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

            //--make sure the buttons are only added to the dictionary once
            if (!playerButtons.ContainsKey(button.Name))
            {
                playerButtons.Add(button.Name, button);
            }

        }

        private void MutationPointMessage_Loaded(object sender, RoutedEventArgs e)
        {
            var mutationPointMessageTextBlock = sender as TextBlock;
            var player = mutationPointMessageTextBlock.DataContext as IPlayer;
            _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerNumber] = mutationPointMessageTextBlock;
        }

        private void SkillTreeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;
            _playerNumberToSkillTreeDialog[player.PlayerNumber] = skillTreeDialog;
        }

        private async void SkillTreeButton_OnClick(object sender, RoutedEventArgs e)
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

        private void Exit_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Application.Current.Exit();
        }

        private async void PlayAgain_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var result =
                await CoreApplication.RequestRestartAsync(string.Empty);
            if (result == AppRestartFailureReason.NotInForeground ||
                result == AppRestartFailureReason.RestartPending ||
                result == AppRestartFailureReason.Other)
            {
                Debug.WriteLine("RequestRestartAsync failed: {0}", result);
            }
        }
    }
}
