using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ApiClient;
using ApiClient.Models;
using Logic;
using Microsoft.Toolkit.Uwp.UI.Animations;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FungusToast
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public FungusToastViewModel ViewModel { get; set; }

        private readonly AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Colors.White
        };

        private readonly Dictionary<string, Dictionary<string, Button>> _playerNumberToMutationButtons = new Dictionary<string, Dictionary<string, Button>>();
        private readonly Dictionary<string, TextBlock> _playerNumberToMutationPointAnnouncementTextBlock = new Dictionary<string, TextBlock>();
        private readonly Dictionary<string, ContentDialog> _playerNumberToSkillTreeDialog = new Dictionary<string, ContentDialog>();
        private readonly Dictionary<string, Button> _playerNumberToSkillTreeButton = new Dictionary<string, Button>();
        private readonly Dictionary<string, SolidColorBrush> _playerNumberToColorBrushDictionary = new Dictionary<string, SolidColorBrush>();

        private char _deadCellSymbol = '☠';

        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);
        private readonly Thickness _activeThickness = new Thickness(8);

        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);

        //--TODO introduce dependency injection framework
        //private ICellGrowthCalculator _cellGrowthCalculator;
        //private ICellRegrowthCalculator _cellRegrowthCalculator;
        //private ISurroundingCellCalculator _surroundingCellCalculator;
        //private GenerationAdvancer _generationAdvancer;

        private IFungusToastApiClient _fungusToastApiClient;

        private ApplicationDataContainer _applicationDataContainer =
            Windows.Storage.ApplicationData.Current.LocalSettings;
        private Windows.Storage.StorageFolder _localFolder =
            Windows.Storage.ApplicationData.Current.LocalFolder;

        private GameModel _gameModel;


        public MainPage()
        {
            InitializeDependencies();
            InitializeComponent();
            ViewModel = new FungusToastViewModel();
        }

        private void InitializeDependencies()
        {
            //_cellGrowthCalculator = new CellGrowthCalculator();
            //_surroundingCellCalculator = new SurroundingCellCalculator(GameSettings.NumberOfColumnsAndRows);
            //_cellRegrowthCalculator = new CellRegrowthCalculator(_surroundingCellCalculator);
            //_generationAdvancer = new GenerationAdvancer(_cellRegrowthCalculator);
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

        private string _userName = "jake";

        private async Task GameStart_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var players = new List<IPlayer>();
            var numberOfHumanPlayers = int.Parse(NumberOfHumanPlayersComboBox.SelectedValue.ToString());
            var numberOfAiPlayers = int.Parse(NumberOfAiPlayersComboBox.SelectedValue.ToString());

            var newGameRequest = new NewGameRequest(_userName, numberOfHumanPlayers, numberOfAiPlayers);
            _gameModel = await _fungusToastApiClient.CreateGame(newGameRequest);
            for (int i = 1; i <= _gameModel.Players.Count; i++)
            {
                players.Add(new Player(_gameModel.Players[i].Name, _availableColors[i-1], _gameModel.Players[i].Id, _gameModel.Players[i].Human));
            }

            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerId, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerId, new Dictionary<string, Button>());
            }

            await InitializeToastWithPlayerCells();
        }

        private async Task InitializeToastWithPlayerCells()
        {
            Toast.Columns = _gameModel.NumberOfColumns;
            Toast.Rows = _gameModel.NumberOfRows;

            //--make the grid a square since it wasn't doing that for some reason
            Toast.Width = Toast.ActualHeight;

            InitializeToast();

            await UpdateToast();
        }

        private void InitializeToast()
        {
            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            Thickness noPaddingOrMargin = new Thickness(0);
            var previousGameState = _gameModel.PreviousGameModel;

            for (var i = 0; i < _gameModel.NumberOfCells; i++)
            {
                Toast.Children.Add(new Button
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
        }

        private async Task UpdateToast()
        {
            foreach (var growthCycle in _gameModel.GrowthCycles)
            {
                foreach (var toastChange in growthCycle.ToastChanges)
                {
                    await RenderToastChange(toastChange);
                }

                foreach (var mutationPointEarned in growthCycle.MutationPointsEarned)
                {
                    await RenderMutationPointEarned(mutationPointEarned);
                }
            }
        }

        private async Task RenderToastChange(ToastChange toastChange)
        {
            var currentGridCell = Toast.Children[toastChange.CellIndex] as Button;
            currentGridCell.Opacity = 0;

            if (toastChange.Dead)
            {
                currentGridCell.Background = _deadCellBrush;
                currentGridCell.Content = _deadCellSymbol;
            }
            else
            {
                currentGridCell.Background = _playerNumberToColorBrushDictionary[toastChange.PlayerId];
                currentGridCell.Content = string.Empty;
            }

            await currentGridCell.Fade(0, 500).StartAsync();
        }

        private async Task RenderMutationPointEarned(KeyValuePair<string, int> mutationPointEarned)
        {
            var mutationPointAnnouncementMessage =
                _playerNumberToMutationPointAnnouncementTextBlock[mutationPointEarned.Key];
            mutationPointAnnouncementMessage.Text = $"+{mutationPointEarned.Value} Mutation Point!";

            mutationPointAnnouncementMessage.Opacity = 1;
            await mutationPointAnnouncementMessage.Fade(0, 500).StartAsync();
        }

        //private async void NextGenerationCycle()
        //{
        //    for (int i = 0; i < ViewModel.NumberOfGenerationsBetweenFreeMutations; i++)
        //    {
        //        await NextGeneration();
        //    }

        //    foreach (var player in ViewModel.Players)
        //    {
        //        await IncreasePlayerMutationPoints(player);
        //    }

        //    //--only allow points to be spent every X rounds
        //    PromptForMutationChoice();
        //}

        //private async Task NextGeneration()
        //{
        //    GrowButton.IsEnabled = false;

        //    List<IPlayer> players = ViewModel.Players;

        //    var nextGenerationResult = await Task.Run(() => _generationAdvancer.NextGeneration(ViewModel.CurrentLiveCells, ViewModel.CurrentDeadCells,
        //        players));

        //    AddNewCells(nextGenerationResult.NewLiveCells);

        //    KillCells(nextGenerationResult.NewDeadCells);

        //    RegrowCells(nextGenerationResult.RegrownCells);

        //    ViewModel.GenerationNumber++;

        //    foreach (var player in players)
        //    {
        //        var playerGrowthSummary = nextGenerationResult.PlayerGrowthSummaries[player.PlayerNumber];

        //        player.LiveCells += playerGrowthSummary.NewLiveCellCount;
        //        player.LiveCells += nextGenerationResult.PlayerNumberToNumberOfRegrownCells[player.PlayerNumber];
        //        player.LiveCells -= playerGrowthSummary.NewDeadCellCount;

        //        player.DeadCells += playerGrowthSummary.NewDeadCellCount;
        //        player.DeadCells -= nextGenerationResult.PlayerNumberToNumberOfDeadCellsEliminated[player.PlayerNumber];

        //        var numberOfRegrownCells = nextGenerationResult.PlayerNumberToNumberOfRegrownCells[player.PlayerNumber];
        //        player.RegrownCells += numberOfRegrownCells;

        //        if (player.GetsFreeMutation())
        //        {
        //            await IncreasePlayerMutationPoints(player);
        //        }
        //    }

        //    //--since it's not a spending round we can keep the grow button enabled
        //    GrowButton.IsEnabled = true;

        //    await CheckForGameEnd();
        //}

        //private void AddNewCells(List<BioCell> newLiveCells)
        //{
        //    foreach (var newCell in newLiveCells)
        //    {
        //        //--its possible for two different cells to split to the same cell. For now, the first cell wins
        //        if (!ViewModel.CurrentLiveCells.ContainsKey(newCell.CellIndex))
        //        {
        //            var button = Toast.Children[newCell.CellIndex] as Button;
        //            button.Background = _playerNumberToColorBrushDictionary[newCell.Player.PlayerNumber];
        //            //button.Content = newCell.Player.PlayerSymbol;
        //            ViewModel.AddNewLiveCell(newCell);
        //        }
        //    }
        //}

        //private void KillCells(List<BioCell> newDeadCells)
        //{
        //    foreach (var newDeadCell in newDeadCells)
        //    {
        //        if (!ViewModel.CurrentDeadCells.ContainsKey(newDeadCell.CellIndex))
        //        {
        //            var button = Toast.Children[newDeadCell.CellIndex] as Button;
        //            button.Background = _deadCellBrush;
        //            button.Content = _deadCellSymbol;
        //            ViewModel.AddNewDeadCell(newDeadCell);
        //        }

        //        ViewModel.RemoveLiveCell(newDeadCell.CellIndex);
        //    }
        //}

        //private void RegrowCells(List<BioCell> regrownCells)
        //{
        //    foreach (var regrownCell in regrownCells)
        //    {
        //        ViewModel.RegrowCell(regrownCell);

        //        var element = Toast.Children[regrownCell.CellIndex] as Button;
        //        element.Background = _playerNumberToColorBrushDictionary[regrownCell.Player.PlayerNumber];
        //        element.Content = string.Empty;
        //    }
        //}

        //private async void Grow_OnClick(object sender, RoutedEventArgs e)
        //{
        //    NextGenerationCycle();
        //    var growButton = sender as Button;
        //    growButton.Visibility = Visibility.Collapsed;
        //}

        //private async Task CheckForGameEnd()
        //{
        //    if (ViewModel.TotalEmptyCells == 0)
        //    {
        //        if (ViewModel.GameEndCountDown == 0)
        //        {
        //            ViewModel.TriggerGameOverResultOnPropertyChanged();
        //            await GameEndContentDialog.ShowAsync();
        //        }
        //        else
        //        {
        //            ViewModel.GameEndCountDown--;
        //        }
        //    }
        //}

        //private bool MutationConsumptionRound()
        //{
        //    return ViewModel.GenerationNumber % ViewModel.NumberOfGenerationsBetweenFreeMutations == 0;
        //}

        //private async Task IncreasePlayerMutationPoints(IPlayer player)
        //{
        //    player.AvailableMutationPoints++;
        //    var mutationPointAnnouncementMessage =
        //        _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerNumber];

        //    mutationPointAnnouncementMessage.Opacity = 1;
        //    mutationPointAnnouncementMessage.Fade(0, 2500).StartAsync();
        //}

        private void PromptForMutationChoice()
        {
            foreach (var player in ViewModel.Players)
            {
                if (player.IsCurrentPlayer(_userName))
                {
                    var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerId];
                    skillTreeButton.BorderBrush = _activeBorderBrush;
                    skillTreeButton.BorderThickness = _activeThickness;
                    EnablePlayerMutationButtons(player);
                }
            }
        }

        //private void MakeAiTurn(IAiPlayer aiPlayer)
        //{
        //    switch (aiPlayer.AiType)
        //    {
        //        case AiType.ExtremeGrowth:
        //            while (aiPlayer.AvailableMutationPoints > 0)
        //            {
        //                if (aiPlayer.TopLeftGrowthChance < 50)
        //                {
        //                    aiPlayer.IncreaseCornerGrowth();
        //                }
        //                else if(aiPlayer.GrowthScorecard.ApoptosisChancePercentage > 0)
        //                {
        //                    aiPlayer.DecreaseApoptosisChance();
        //                }
        //                else
        //                {
        //                    aiPlayer.IncreaseRegrowthChance();
        //                }
        //            }

        //            break;

        //        case AiType.Random:
        //            while (aiPlayer.AvailableMutationPoints > 0)
        //            {
        //                var mutationChoiceIndex = RandomNumberGenerator.Random.Next(0, 3);
        //                switch (mutationChoiceIndex)
        //                {
        //                    case 0:
        //                        aiPlayer.IncreaseMutationChance();
        //                        break;
        //                    case 1:
        //                        aiPlayer.IncreaseCornerGrowth();
        //                        break;
        //                    case 2:
        //                        aiPlayer.IncreaseRegrowthChance();
        //                        break;
        //                    case 3:
        //                        if (aiPlayer.GrowthScorecard.ApoptosisChancePercentage > 0)
        //                        {
        //                            aiPlayer.DecreaseApoptosisChance();
        //                        }
        //                        else
        //                        {
        //                            aiPlayer.IncreaseRegrowthChance();
        //                        }
        //                        break;
        //                }
        //            }

        //            break;
        //        default:
        //            throw new Exception("Unexpected AiType: " + aiPlayer.AiType);
        //    }
        //}

        private void CheckForRemainingMutationPoints(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
            {
                return;
            }

            DisablePlayerMutationButtons(player);

            //TODO this is where skill expenditures should be sent
            //_fungusToastApiClient.PushSkillExpenditures()
        }

        private void IncreaseMutationChance_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;

            player.IncreaseMutationChance();

            CheckForRemainingMutationPoints(player);
        }

        private void AntiApoptosis_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.DecreaseApoptosisChance();
            if (player.GrowthScorecard.ApoptosisChancePercentage <= 0)
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
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerId];
            foreach (var mutationButton in playerMutationButtons)
            {
                if (mutationButton.Key == "AntiApoptosisButton" && player.GrowthScorecard.ApoptosisChancePercentage <= 0)
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
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerId];

            foreach (var button in playerMutationButtons)
            {
                button.Value.IsEnabled = false;
            }

            var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerId];
            skillTreeButton.BorderBrush = _normalBorderBrush;
            skillTreeButton.BorderThickness = _normalThickness;

            var dialog = _playerNumberToSkillTreeDialog[player.PlayerId];
            dialog.Hide();
        }
        
        private void MutationOptionButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var playerButtons = _playerNumberToMutationButtons[player.PlayerId];

            if (player.AvailableMutationPoints > 0 && player.IsCurrentPlayer(_userName))
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
            _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerId] = mutationPointMessageTextBlock;
        }

        private void SkillTreeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;
            _playerNumberToSkillTreeDialog[player.PlayerId] = skillTreeDialog;
        }

        private async void SkillTreeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var contentDialog = _playerNumberToSkillTreeDialog[player.PlayerId];
            await contentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void SkillTreeButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            _playerNumberToSkillTreeButton[player.PlayerId] = button;
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
