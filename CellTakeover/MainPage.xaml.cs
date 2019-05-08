﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using ApiClient;
using ApiClient.Models;
using ApiClient.Serialization;
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

        //TODO this is hard-coded until we get authentication working
        private readonly string _userName = MockDataBuilder.AppUserName;

        private readonly AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Colors.White
        };

        private readonly Dictionary<string, Dictionary<string, Button>> _playerNumberToMutationButtons = new Dictionary<string, Dictionary<string, Button>>();
        private readonly Dictionary<string, TextBlock> _playerNumberToMutationPointAnnouncementTextBlock = new Dictionary<string, TextBlock>();
        private readonly Dictionary<string, ContentDialog> _playerNumberToSkillTreeDialog = new Dictionary<string, ContentDialog>();
        private readonly Dictionary<string, Button> _playerNumberToSkillTreeButton = new Dictionary<string, Button>();
        private readonly Dictionary<string, SolidColorBrush> _playerNumberToColorBrushDictionary = new Dictionary<string, SolidColorBrush>();

        /// <summary>
        /// If the skill tree dialog is open then this will have a reference to it
        /// </summary>
        private ContentDialog _visibleDialog;

        private char _deadCellSymbol = '☠';

        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);
        private readonly Thickness _activeThickness = new Thickness(8);

        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);
        private IFungusToastApiClient _fungusToastApiClient;

        private readonly ApplicationDataContainer _applicationDataContainer = ApplicationData.Current.LocalSettings;
        private const string SettingsContainerName = "SettingsContainer";
        private const string ActiveGameIdSetting = "ActiveGameId";
        private readonly ApplicationDataContainer _settingsDataContainer;

        private GameModel _gameModel;

        private SkillExpenditureRequest _skillExpenditureRequest = new SkillExpenditureRequest();

        private bool _playersListViewLoaded = false;

        public MainPage()
        {
            InitializeDependencies();
            InitializeComponent();
            ViewModel = new FungusToastViewModel();
            _settingsDataContainer =
                _applicationDataContainer.CreateContainer(SettingsContainerName,
                    ApplicationDataCreateDisposition.Always);
        }

        private void InitializeDependencies()
        {
            _fungusToastApiClient = new FungusToastApiClient(GameSettings.BaseURL, new GamesApiClient(new Serializer()));
        }


        private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //--if there is an active game then load that, otherwise prompt to start a new game
            if (_settingsDataContainer.Values.TryGetValue(ActiveGameIdSetting, out var activeGameId))
            {
                _gameModel = await _fungusToastApiClient.GetGameState(int.Parse(activeGameId.ToString()));
                InitializeGame(_gameModel);
            }
            else
            {
                await GameSettingsDialog.ShowAsync();
            }
        }

        private readonly List<Color> _availableColors = new List<Color>
        {
            Colors.SkyBlue,
            Colors.PaleVioletRed,
            Colors.Blue,
            Colors.Orange,
            Colors.Gray,
            Colors.OliveDrab
        };

        private async void GameStart_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var numberOfHumanPlayers = int.Parse(NumberOfHumanPlayersComboBox.SelectedValue.ToString());
            var numberOfAiPlayers = int.Parse(NumberOfAiPlayersComboBox.SelectedValue.ToString());

            if (numberOfHumanPlayers + numberOfAiPlayers < 7)
            {
                var newGameRequest = new NewGameRequest(_userName, numberOfHumanPlayers, numberOfAiPlayers);
                _gameModel = await _fungusToastApiClient.CreateGame(newGameRequest, false);
                _settingsDataContainer.Values[ActiveGameIdSetting] = _gameModel.Id;

                InitializeGame(_gameModel);
            }
            else
            {
                //TODO provide message if adding too many players   
            }
        }

        private async void InitializeGame(GameModel game)
        {
            var players = new List<IPlayer>();
            var reorderedPlayers = game.Players
                .OrderByDescending(player => player.Name == _userName)
                .ThenBy(player => player.Human)
                .ThenBy(player => player.Id)
                .ToList();
            for (var i = 1; i <= reorderedPlayers.Count; i++)
            {
                var playerState = reorderedPlayers[i - 1];
                var player = new Player(playerState.Name, _availableColors[i - 1], playerState.Id,
                    playerState.Human);
                UpdatePlayer(player, playerState);
                players.Add(player);
            }

            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerId, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerId, new Dictionary<string, Button>());
            }

            InitializeToastWithPlayerCells(game);

            SetGameStats(game);
            await CheckForGameEnd();
        }

        private void InitializeToastWithPlayerCells(GameModel game)
        {
            Toast.Columns = _gameModel.GridSize;
            Toast.Rows = _gameModel.GridSize;

            //--make the grid a square since it wasn't doing that for some reason
            Toast.Width = Toast.ActualHeight;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            var noPaddingOrMargin = new Thickness(0);
            var previousGameState = game.StartingGameState;

            for (var i = 0; i < game.NumberOfCells; i++)
            {
                Brush backgroundBrush;
                char gridCellContent = ' ';
                if (previousGameState.CellsDictionary.ContainsKey(i))
                {
                    var cell = previousGameState.CellsDictionary[i];
                    if (!cell.Live)
                    {
                        backgroundBrush = _deadCellBrush;
                        gridCellContent = _deadCellSymbol;
                    }
                    else
                    {
                        backgroundBrush = _playerNumberToColorBrushDictionary[cell.PlayerId];
                    }
                }
                else
                {
                    backgroundBrush = _emptyCellBrush;
                }

                Toast.Children.Add(new Button
                {
                    Style = Resources["ButtonRevealStyle"] as Style,
                    Background = backgroundBrush,
                    Content = gridCellContent,
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

        private async Task RenderUpdates(GameModel game)
        {
            List<Task> tasks = new List<Task>();
            foreach (var growthCycle in game.GrowthCycles)
            {
                foreach (var toastChange in growthCycle.ToastChanges)
                {
                    tasks.Add(RenderToastChange(toastChange));
                }

                foreach (var mutationPointEarned in growthCycle.MutationPointsEarned)
                {
                    tasks.Add(RenderMutationPointEarned(mutationPointEarned));
                }

                foreach (var task in tasks)
                {
                    await task;
                }

                tasks = new List<Task>();

                ViewModel.GenerationNumber = growthCycle.GenerationNumber;
            }

            foreach (var playerState in game.Players)
            {
                var matchingPlayer = ViewModel.Players.FirstOrDefault(x => x.PlayerId == playerState.Id);
                if (matchingPlayer == null)
                {
                    throw new InvalidOperationException($"Couldn't find a player in this game with id '{playerState.Id}'");
                }

                UpdatePlayer(matchingPlayer, playerState);
            }

            SetGameStats(game);
        }

        private void SetGameStats(GameModel game)
        {
            ViewModel.RoundNumber = game.RoundNumber;
            ViewModel.GenerationNumber = game.GenerationNumber;
            ViewModel.TotalDeadCells = game.TotalDeadCells;
            ViewModel.TotalEmptyCells = game.TotalEmptyCells;
            ViewModel.TotalLiveCells = game.TotalLiveCells;
            ViewModel.TotalRegeneratedCells = game.TotalRegeneratedCells;
            if (game.EndOfGameCountDown.HasValue)
            {
                ViewModel.GameEndCountDown = game.EndOfGameCountDown.Value;
                EndOfGameCountDownLabel.Visibility = Visibility.Visible;
                EndOfGameCountDownTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePlayer(IPlayer playerToUpdate, PlayerState playerStateValuesToCopy)
        {
            //--zero out AI players' mutation points since they always spend them immediately
            if (!playerToUpdate.IsHuman)
            {
                playerToUpdate.AvailableMutationPoints = 0;
            }
            playerToUpdate.DeadCells = playerStateValuesToCopy.DeadCells;
            playerToUpdate.LiveCells = playerStateValuesToCopy.LiveCells;
            playerToUpdate.RegrownCells = playerStateValuesToCopy.RegeneratedCells;
            playerToUpdate.GrownCells = playerStateValuesToCopy.GrownCells;
            playerToUpdate.PerishedCells = playerStateValuesToCopy.PerishedCells;
            playerToUpdate.FungicidalKills = playerStateValuesToCopy.FungicidalKills;
            playerToUpdate.SpentMutationPoints = playerStateValuesToCopy.SpentMutationPoints;

            playerToUpdate.HyperMutationSkillLevel = playerStateValuesToCopy.HyperMutationSkillLevel;
            playerToUpdate.AntiApoptosisSkillLevel = playerStateValuesToCopy.AntiApoptosisSkillLevel;
            playerToUpdate.RegenerationSkillLevel = playerStateValuesToCopy.RegenerationSkillLevel;
            playerToUpdate.BuddingSkillLevel = playerStateValuesToCopy.BuddingSkillLevel;
            playerToUpdate.MycotoxinsSkillLevel = playerStateValuesToCopy.MycotoxinsSkillLevel;

            var updatedGrowthScorecard = new GrowthScorecard
            {
                ApoptosisChancePercentage = playerStateValuesToCopy.ApoptosisChance,
                StarvedCellDeathChancePercentage = playerStateValuesToCopy.StarvedCellDeathChance,
                MutationChancePercentage = playerStateValuesToCopy.MutationChance,
                RegenerationChancePercentage = playerStateValuesToCopy.RegenerationChance,
                MycotoxinFungicideChancePercentage = playerStateValuesToCopy.MycotoxinFungicideChance,
                GrowthChanceDictionary = new Dictionary<RelativePosition, float>
                {
                    {RelativePosition.TopLeft, playerStateValuesToCopy.TopLeftGrowthChance},
                    {RelativePosition.Top, playerStateValuesToCopy.TopGrowthChance},
                    {RelativePosition.TopRight, playerStateValuesToCopy.TopRightGrowthChance},
                    {RelativePosition.Right, playerStateValuesToCopy.RightGrowthChance},
                    {RelativePosition.BottomRight, playerStateValuesToCopy.BottomRightGrowthChance},
                    {RelativePosition.Bottom, playerStateValuesToCopy.BottomGrowthChance},
                    {RelativePosition.BottomLeft, playerStateValuesToCopy.BottomLeftGrowthChance},
                    {RelativePosition.Left, playerStateValuesToCopy.LeftGrowthChance},
                }
            };

            playerToUpdate.GrowthScorecard = updatedGrowthScorecard;

            if (playerToUpdate.IsCurrentPlayer(_userName) && playerToUpdate.AvailableMutationPoints > 0)
            {
                var skillTreeButton = _playerNumberToSkillTreeButton[playerToUpdate.PlayerId];
                skillTreeButton.BorderBrush = _activeBorderBrush;
                skillTreeButton.BorderThickness = _activeThickness;
            }
        }

        private async Task RenderToastChange(ToastChange toastChange)
        {
            var currentGridCell = Toast.Children[toastChange.Index] as Button;
            currentGridCell.Opacity = 0;

            if (toastChange.Live)
            {
                currentGridCell.Background = _playerNumberToColorBrushDictionary[toastChange.PlayerId];
                currentGridCell.Content = string.Empty;
            }
            else
            {
                currentGridCell.Background = _deadCellBrush;
                currentGridCell.Content = _deadCellSymbol;
            }

            await currentGridCell.Fade(1, 1500).StartAsync();
        }

        private async Task RenderMutationPointEarned(KeyValuePair<string, int> mutationPointEarned)
        {
            if (mutationPointEarned.Value > 0)
            {
                var mutationPointAnnouncementMessage =
                    _playerNumberToMutationPointAnnouncementTextBlock[mutationPointEarned.Key];
                mutationPointAnnouncementMessage.Text = $"+{mutationPointEarned.Value} Mutation Point!";
                ViewModel.Players.First(player => player.PlayerId == mutationPointEarned.Key).AvailableMutationPoints +=
                    mutationPointEarned.Value;

                mutationPointAnnouncementMessage.Opacity = 1;
                await mutationPointAnnouncementMessage.Fade(0, 1500).StartAsync();
            }
        }

        private void EnableMutationButtons(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
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
        }

        private async Task CheckForRemainingMutationPoints(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
            {
                return;
            }

            DisablePlayerMutationButtons(player);
            
            var skillUpdateResult = await _fungusToastApiClient.PushSkillExpenditures(_gameModel.Id, player.PlayerId, _skillExpenditureRequest);

            _skillExpenditureRequest = new SkillExpenditureRequest();

            if (skillUpdateResult.NextRoundAvailable)
            {
                _gameModel = await _fungusToastApiClient.GetGameState(_gameModel.Id);

                await RenderUpdates(_gameModel);

                if (await CheckForGameEnd())
                {
                    EnableMutationButtons(player);
                }
            }
        }

        private async void IncreaseHypermutation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;

            player.IncreaseHypermutation();
            _skillExpenditureRequest.HypermutationPoints++;

            await CheckForRemainingMutationPoints(player);
        }

        private async void AntiApoptosis_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.DecreaseApoptosisChance();
            _skillExpenditureRequest.AntiApoptosisPoints++;
            if (player.GrowthScorecard.ApoptosisChancePercentage <= 0)
            {
                button.IsEnabled = false;
            }

            await CheckForRemainingMutationPoints(player);
        }

        private async void IncreaseBudding_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseBudding();
            _skillExpenditureRequest.BuddingPoints++;

            await CheckForRemainingMutationPoints(player);
        }

        private async void IncreaseRegeneration_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseRegeneration();
            _skillExpenditureRequest.RegenerationPoints++;

            await CheckForRemainingMutationPoints(player);
        }

        private async void MycotoxinFungicideButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseMycotoxicity();
            _skillExpenditureRequest.MycotoxicityPoints++;

            await CheckForRemainingMutationPoints(player);
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

        private async void MutationPointMessage_Loaded(object sender, RoutedEventArgs e)
        {
            var mutationPointMessageTextBlock = sender as TextBlock;
            var playerId = mutationPointMessageTextBlock.Tag.ToString();
            _playerNumberToMutationPointAnnouncementTextBlock[playerId] = mutationPointMessageTextBlock;

            if (_gameModel != null && _playerNumberToMutationPointAnnouncementTextBlock.Keys.Count == _gameModel.Players.Count)
            {
                await RenderUpdates(_gameModel);
            }
        }

        private void SkillTreeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var skillTreeDialog = sender as ContentDialog;
            var playerId = skillTreeDialog.Tag.ToString();
            _playerNumberToSkillTreeDialog[playerId] = skillTreeDialog;
        }

        private void SkillTreeDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;

            if (player.IsCurrentPlayer(_userName))
            {
                EnableMutationButtons(player);
            }
        }
        private void SkillTreeDialog_OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            _visibleDialog = null;
        }

        private async void SkillTreeButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var contentDialog = _playerNumberToSkillTreeDialog[player.PlayerId];
            _visibleDialog = contentDialog;
            await contentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void SkillTreeButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var playerId = button.Tag.ToString();
            //var player = button.DataContext as IPlayer;
            _playerNumberToSkillTreeButton[playerId] = button;
        }

        private async Task<bool> CheckForGameEnd()
        {
            if (_gameModel.Status == GameStatus.Finished)
            {
                _visibleDialog?.Hide();
                await GameEndContentDialog.ShowAsync();
                return true;
            }

            return false;
        }

        private void Exit_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ClearGame();
            Application.Current.Exit();
        }

        private async void PlayAgain_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            ClearGame();
            await RestartApp();
        }

        private static async Task RestartApp()
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

        private void ClearExistingGame_OnClick(object sender, RoutedEventArgs e)
        {
            ClearGame();
            Application.Current.Exit();
        }

        private void ClearGame()
        {
            _settingsDataContainer.Values.Remove(ActiveGameIdSetting);
        }

        public const int MaxPlayers = 6;
        public ObservableCollection<int> NumberOfHumanPlayersOptions = new ObservableCollection<int>
        {
            1,
            2,
            3,
            4,
            5,
            6
        };

        public ObservableCollection<int> NumberOfAiPlayersOptions = new ObservableCollection<int>
        {
            1,
            2,
            3,
            4,
            5
        };

        private void NumberOfHumanPlayersComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            (sender as ComboBox).SelectedItem = 1;
        }

        private void NumberOfHumanPlayersComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            var numberOfHumanPlayers = (int)comboBox.SelectedItem;
           
            var numberOfAiPlayersAllowed = MaxPlayers - numberOfHumanPlayers;

            NumberOfAiPlayersOptions.Clear();
            var previouslySelectedValue = (int) (NumberOfAiPlayersComboBox.SelectedItem ?? 0);

            for (int i = 0; i <= numberOfAiPlayersAllowed; i++)
            {
                NumberOfAiPlayersOptions.Add(i);
            }

            if (numberOfAiPlayersAllowed >= previouslySelectedValue)
            {
                NumberOfAiPlayersComboBox.SelectedItem = previouslySelectedValue;
            }
            else
            {
                NumberOfAiPlayersComboBox.SelectedItem = NumberOfAiPlayersOptions.Last();
            }
        }

        private void NumberOfAiPlayersComboBox_OnLoaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox.SelectedItem == null)
            {
                comboBox.SelectedItem = 0;
            }
        }
    }
}
