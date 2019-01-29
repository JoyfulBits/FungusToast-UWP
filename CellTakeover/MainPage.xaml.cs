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

        private string _userName = "jake";

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
            //--if there is an active game then load that, otherwise prompt to start a new game
            if(int.TryParse(_applicationDataContainer.Values["activeGameId"].ToString(), out var activeGameId))
            {
                _gameModel = await _fungusToastApiClient.GetGameState(activeGameId);

                await InitializeGame(_gameModel);
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
            _gameModel = await _fungusToastApiClient.CreateGame(newGameRequest);

            await InitializeGame(_gameModel);
        }

        private async Task InitializeGame(GameModel game)
        {
            var players = new List<IPlayer>();

            for (var i = 1; i <= game.Players.Count; i++)
            {
                players.Add(new Player(game.Players[i].Name, _availableColors[i - 1], game.Players[i].Id,
                    game.Players[i].Human));
            }

            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerId, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerId, new Dictionary<string, Button>());
            }

            InitializeToastWithPlayerCells(game);

            await RenderUpdates(game);
        }

        private void InitializeToastWithPlayerCells(GameModel game)
        {
            Toast.Columns = _gameModel.NumberOfColumns;
            Toast.Rows = _gameModel.NumberOfRows;

            //--make the grid a square since it wasn't doing that for some reason
            Toast.Width = Toast.ActualHeight;

            var blackSolidColorBrush = new SolidColorBrush(Colors.Black);
            var noPaddingOrMargin = new Thickness(0);
            var previousGameState = game.PreviousGameModel;

            for (var i = 0; i < game.NumberOfCells; i++)
            {
                Brush backgroundBrush;
                if (previousGameState.Cells.ContainsKey(i))
                {
                    var cell = previousGameState.Cells[i];
                    backgroundBrush = _playerNumberToColorBrushDictionary[cell.PlayerId];
                }
                else
                {
                    backgroundBrush = _emptyCellBrush;
                }

                Toast.Children.Add(new Button
                {
                    Style = Resources["ButtonRevealStyle"] as Style,
                    Background = backgroundBrush,
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
            foreach (var growthCycle in game.GrowthCycles)
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
