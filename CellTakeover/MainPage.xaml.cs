using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private char _deadCellSymbol = '☠';

        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);
        private readonly Thickness _activeThickness = new Thickness(8);

        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Black);
        private readonly Thickness _normalThickness = new Thickness(1);
        private IFungusToastApiClient _fungusToastApiClient;

        private readonly ApplicationDataContainer _applicationDataContainer = ApplicationData.Current.LocalSettings;

        private GameModel _gameModel;

        private SkillExpenditureRequest _skillExpenditureRequest = new SkillExpenditureRequest();

        private bool _mainGridLoaded = false;
        private bool _playersListViewLoaded = false;
        private bool _gameLoaded = false;

        public MainPage()
        {
            InitializeDependencies();
            InitializeComponent();
            ViewModel = new FungusToastViewModel();
        }

        private void InitializeDependencies()
        {
            _fungusToastApiClient = new FungusToastApiClient(GameSettings.BaseURL, new GamesApiClient(new Serializer()));
        }


        private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _mainGridLoaded = true;
            //--if there is an active game then load that, otherwise prompt to start a new game
            if (_applicationDataContainer.Values.TryGetValue("activeGameId", out var activeGameId))
            {
                _gameModel = await _fungusToastApiClient.GetGameState(int.Parse(activeGameId.ToString()));//, MockOption.AdvancedGame);
                _gameLoaded = true;
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
            Colors.Gray
        };

        private async void GameStart_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var numberOfHumanPlayers = int.Parse(NumberOfHumanPlayersComboBox.SelectedValue.ToString());
            var numberOfAiPlayers = int.Parse(NumberOfAiPlayersComboBox.SelectedValue.ToString());

            var newGameRequest = new NewGameRequest(_userName, numberOfHumanPlayers, numberOfAiPlayers);
            _gameModel = await _fungusToastApiClient.CreateGame(newGameRequest, false);
            _applicationDataContainer.Values["activeGameId"] = _gameModel.Id;

            InitializeGame(_gameModel);
        }

        private void InitializeGame(GameModel game)
        {
            var players = new List<IPlayer>();
            for (var i = 1; i <= game.Players.Count; i++)
            {
                var playerState = game.Players[i - 1];
                var player = new Player(playerState.Name, _availableColors[i - 1], playerState.Id,
                    playerState.Human)
                {
                    AvailableMutationPoints = playerState.MutationPoints
                };
                players.Add(player);
            }

            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerId, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerId, new Dictionary<string, Button>());
            }

            InitializeToastWithPlayerCells(game);
        }

        private void InitializeToastWithPlayerCells(GameModel game)
        {
            Toast.Columns = _gameModel.NumberOfColumns;
            Toast.Rows = _gameModel.NumberOfRows;

            //--make the grid a square since it wasn't doing that for some reason
            Toast.Width = Toast.ActualHeight;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            var noPaddingOrMargin = new Thickness(0);
            var previousGameState = game.PreviousGameState;

            for (var i = 0; i < game.NumberOfCells; i++)
            {
                Brush backgroundBrush;
                char gridCellContent = ' ';
                if (previousGameState.CellsDictionary.ContainsKey(i))
                {
                    var cell = previousGameState.CellsDictionary[i];
                    if (cell.Dead)
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

            ViewModel.RoundNumber = game.RoundNumber;

            foreach (var playerState in game.Players)
            {
                var matchingPlayer = ViewModel.Players.FirstOrDefault(x => x.PlayerId == playerState.Id);
                if (matchingPlayer == null)
                {
                    throw new InvalidOperationException($"Couldn't find a player in this game with id '{playerState.Id}'");
                }

                matchingPlayer.AvailableMutationPoints = playerState.MutationPoints;
                matchingPlayer.DeadCells = playerState.DeadCells;
                matchingPlayer.LiveCells = playerState.LiveCells;
                matchingPlayer.RegrownCells = playerState.RegeneratedCells;

                matchingPlayer.HyperMutationSkillLevel = playerState.HyperMutationSkillLevel;
                matchingPlayer.AntiApoptosisSkillLevel = playerState.AntiApoptosisSkillLevel;
                matchingPlayer.RegenerationSkillLevel = playerState.RegenerationSkillLevel;
                matchingPlayer.BuddingSkillLevel = playerState.BuddingSkillLevel;
                matchingPlayer.MycotoxinsSkillLevel = playerState.MycotoxinsSkillLevel;

                var updatedGrowthScorecard = new GrowthScorecard
                {
                    ApoptosisChancePercentage = playerState.ApoptosisChancePercentage,
                    StarvedCellDeathChancePercentage = playerState.StarvedCellDeathChancePercentage,
                    MutationChancePercentage = playerState.MutationChancePercentage,
                    RegenerationChancePercentage = playerState.RegenerationChancePercentage,
                    MycotoxinFungicideChancePercentage = playerState.MycotoxinFungicideChancePercentage,
                    GrowthChanceDictionary = new Dictionary<RelativePosition, double>
                    {
                        { RelativePosition.TopLeft, playerState.TopLeftGrowthChance },
                        { RelativePosition.Top, playerState.TopGrowthChance },
                        { RelativePosition.TopRight, playerState.TopRightGrowthChance },
                        { RelativePosition.Right, playerState.RightGrowthChance },
                        { RelativePosition.BottomRight, playerState.BottomRightGrowthChance },
                        { RelativePosition.Bottom, playerState.BottomGrowthChance },
                        { RelativePosition.BottomLeft, playerState.BottomLeftGrowthChance },
                        { RelativePosition.Left, playerState.LeftGrowthChance },
                    }
                };

                matchingPlayer.GrowthScorecard = updatedGrowthScorecard;
            }

            ViewModel.GenerationNumber = game.GenerationNumber;
            ViewModel.RoundNumber = game.RoundNumber;
            ViewModel.TotalDeadCells = game.TotalDeadCells;
            ViewModel.TotalEmptyCells = game.TotalEmptyCells;
            ViewModel.TotalLiveCells = game.TotalLiveCells;
            ViewModel.TotalRegeneratedCells = game.TotalRegeneratedCells;
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

            await currentGridCell.Fade(1, 1500).StartAsync();
        }

        private async Task RenderMutationPointEarned(KeyValuePair<string, int> mutationPointEarned)
        {
            if (mutationPointEarned.Value > 0)
            {
                var mutationPointAnnouncementMessage =
                    _playerNumberToMutationPointAnnouncementTextBlock[mutationPointEarned.Key];
                mutationPointAnnouncementMessage.Text = $"+{mutationPointEarned.Value} Mutation Point!";

                mutationPointAnnouncementMessage.Opacity = 1;
                await mutationPointAnnouncementMessage.Fade(0, 1500).StartAsync();
            }
        }

        private void EnableMutationButtons(IPlayer player)
        {
            var skillTreeButton = _playerNumberToSkillTreeButton[player.PlayerId];
            skillTreeButton.BorderBrush = _activeBorderBrush;
            skillTreeButton.BorderThickness = _activeThickness;
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

        private async Task CheckForRemainingMutationPoints(IPlayer player)
        {
            if (player.AvailableMutationPoints > 0)
            {
                return;
            }

            DisablePlayerMutationButtons(player);
            
            var skillUpdateResult = await _fungusToastApiClient.PushSkillExpenditures(_gameModel.Id, player.PlayerId, _skillExpenditureRequest, mockNextRoundAvailable : true);

            _skillExpenditureRequest = new SkillExpenditureRequest();

            if (skillUpdateResult.NextRoundAvailable)
            {
                _gameModel = await _fungusToastApiClient.GetGameState(_gameModel.Id);//, MockOption.AdvancedGame);

                await RenderUpdates(_gameModel);

                EnableMutationButtons(player);
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
            var player = mutationPointMessageTextBlock.DataContext as IPlayer;
            _playerNumberToMutationPointAnnouncementTextBlock[player.PlayerId] = mutationPointMessageTextBlock;

            if (_gameLoaded && _playerNumberToMutationPointAnnouncementTextBlock.Keys.Count == _gameModel.Players.Count)
            {
                await RenderUpdates(_gameModel);
            }
        }

        private async void SkillTreeDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;
            _playerNumberToSkillTreeDialog[player.PlayerId] = skillTreeDialog;

            if (player.IsCurrentPlayer(_userName))
            {
               await CheckForRemainingMutationPoints(player);
            }
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

        private async Task RenderUpdatesIfUiIsReady()
        {
            if (_gameLoaded && _mainGridLoaded && _playerNumberToMutationButtons.Count == _gameModel.Players.Count)
            {
                await RenderUpdates(_gameModel);
            }
        }
    }
}
