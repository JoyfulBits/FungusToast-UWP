﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using ApiClient;
using ApiClient.Exceptions;
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
        private readonly List<string> _validUserNames = new List<string>
        {
            "Human 1",
            "Human 2",
            "Human 3",
            "Human 4",
            "Human 5",
            "Human 6"
        };

        private List<string> _usersPlayingLocalGame = new List<string>();


        private readonly AcrylicBrush _deadCellBrush = new AcrylicBrush
        {
            TintColor = Colors.White
        };

        private readonly AcrylicBrush _moistCellBrush = new AcrylicBrush
        {
            TintColor = Colors.LightGray
        }; 

        private readonly Dictionary<string, Dictionary<string, Button>> _playerNumberToMutationButtons = new Dictionary<string, Dictionary<string, Button>>();
        private readonly Dictionary<string, Dictionary<string, Button>> _playerNumberToActiveSkillsButtons = new Dictionary<string, Dictionary<string, Button>>();
        
        private readonly Dictionary<string, TextBlock> _playerNumberToMutationPointAnnouncementTextBlock = new Dictionary<string, TextBlock>();

        private readonly Dictionary<string, TextBlock> _playerNumberToActionPointAnnouncementTextBlock =
            new Dictionary<string, TextBlock>();
        private readonly Dictionary<string, Border> _playerNumberToPassiveSkillsBorder = new Dictionary<string, Border>();
        private readonly Dictionary<string, Border> _playerNumberToActiveSkillsBorder = new Dictionary<string, Border>();
        private readonly Dictionary<string, ContentDialog> _playerNumberToSkillTreeDialog = new Dictionary<string, ContentDialog>();

        private readonly Dictionary<string, ContentDialog> _playerNumberToActiveSkillsDialog =
            new Dictionary<string, ContentDialog>();
        private readonly Dictionary<string, Button> _playerNumberToPassiveSkillTreeDialogButton = new Dictionary<string, Button>();
        private readonly Dictionary<string, Button> _playerNumberToActiveSkillsDialogButton = new Dictionary<string, Button>();
        private readonly Dictionary<string, SolidColorBrush> _playerNumberToColorBrushDictionary = new Dictionary<string, SolidColorBrush>();

        /// <summary>
        /// If the skill tree dialog is open then this will have a reference to it
        /// </summary>
        private ContentDialog _visibleDialog;

        private char _deadCellSymbol = '☠';

        private readonly SolidColorBrush _emptyCellBrush = new SolidColorBrush(Colors.White);

        private readonly Brush _activeBorderBrush = new SolidColorBrush(Colors.Green);
        private readonly Thickness _activeThickness = new Thickness(5);

        private readonly SolidColorBrush _normalBorderBrush = new SolidColorBrush(Colors.Transparent);
        private readonly Thickness _normalThickness = new Thickness(5);
        private IFungusToastApiClient _fungusToastApiClient;
        private PassiveSkillsData _passiveSkillsData;
        private ActiveSkillsData _activeSkillsData;

        private readonly ApplicationDataContainer _applicationDataContainer = ApplicationData.Current.LocalSettings;
        private const string SettingsContainerName = "SettingsContainer";
        private const string ActiveGameIdSetting = "ActiveGameId";
        private readonly ApplicationDataContainer _settingsDataContainer;

        private GameModel _gameModel;

        private readonly Dictionary<string, SkillExpenditureRequest> _playerToSkillExpenditureRequest = new Dictionary<string, SkillExpenditureRequest>();

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

        private async Task<ActiveSkillsData> InitializeActiveSkillsData()
        {
            var activeSkills = await _fungusToastApiClient.GetActiveSkills();

            var skillIdToActiveSkillData = new Dictionary<int, ActiveSkillData>();

            foreach (var activeSkill in activeSkills)
            {
                var activeSkillData = new ActiveSkillData
                {
                    ActionsPerActionPoint = activeSkill.NumberOfToastChanges,
                    MinimumRound = activeSkill.MinimumRound
                };

                switch (activeSkill.Id)
                {
                    case (int) ActiveSkills.EyeDropper:
                        activeSkillData.Message = $"Add {activeSkillData.ActionsPerActionPoint} water droplets to the toast, making it moist.";
                        break;
                    case (int)ActiveSkills.DeadCell:
                        activeSkillData.Message = $"Add {activeSkillData.ActionsPerActionPoint} dead cell(s) to an empty space on the toast.";
                        break;
                    case (int)ActiveSkills.IncreaseLight:
                        activeSkillData.Message = "Turn up the UV lamp lighting level to decrease all growth by 0.2%";
                        break;
                    case (int)ActiveSkills.DecreaseLight:
                        activeSkillData.Message = "Lower the UV lamp lighting level to increase all growth by 0.2%";
                        break;
                    default:
                        throw new Exception(
                            $"There is a new active skill in the API that is not accounted for in the UWP app. The active skill id is '{activeSkill.Id}' and the name is '{activeSkill.Name}'");
                }

                skillIdToActiveSkillData.Add(activeSkill.Id, activeSkillData);
            }

            var totalExpectedActiveSkills = Enum.GetNames(typeof(ActiveSkills)).Length;

            if (totalExpectedActiveSkills != activeSkills.Count)
            {
                throw new Exception(
                    $"Expected that '{totalExpectedActiveSkills}' active skills would be accounted for, but '{activeSkills.Count}' were set.");
            }

           return new ActiveSkillsData(skillIdToActiveSkillData);
        }

        private async Task<PassiveSkillsData> InitializePassiveSkillsData()
        {
            var passiveSkills = await _fungusToastApiClient.GetPassiveSkills();

            float mutationPercentageChancePerAttributePoint = 0F;
            float cornerGrowthChancePerAttributePoint = 0F;
            float reducedApoptosisPercentagePerAttributePoint = 0F;
            float regenerationChancePerAttributePoint = 0F;
            float mycotoxinFungicideChancePerAttributePoint = 0F;
            float moistureGrowthBoostPerAttributePoint = 0F;
            float sporesChancePerAttributePoint = 0F;

            foreach (var passiveSkill in passiveSkills)
            {
                switch (passiveSkill.Id)
                {
                    case (int) PassiveSkills.AntiApoptosis:
                        reducedApoptosisPercentagePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Budding:
                        cornerGrowthChancePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Hypermutation:
                        mutationPercentageChancePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Mycotoxicity:
                        mycotoxinFungicideChancePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Regeneration:
                        regenerationChancePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Hydrophilia:
                        moistureGrowthBoostPerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    case (int) PassiveSkills.Spores:
                        sporesChancePerAttributePoint = passiveSkill.IncreasePerPoint;
                        break;
                    default:
                        throw new Exception(
                            $"There is a new skill in the API that is not accounted for in the UWP app. The skill id is '{passiveSkill.Id}' and the name is '{passiveSkill.Name}'");
                }
            }

            var totalExpectedPassiveSkills = Enum.GetNames(typeof(PassiveSkills)).Length;

            if (totalExpectedPassiveSkills != passiveSkills.Count)
            {
                throw new Exception(
                    $"Expected that '{totalExpectedPassiveSkills}' passive skills would be accounted for, but '{passiveSkills.Count}' were set.");
            }

            var passiveSkillsData = new PassiveSkillsData(
                mutationPercentageChancePerAttributePoint,
                cornerGrowthChancePerAttributePoint,
                reducedApoptosisPercentagePerAttributePoint,
                regenerationChancePerAttributePoint,
                mycotoxinFungicideChancePerAttributePoint,
                moistureGrowthBoostPerAttributePoint,
                sporesChancePerAttributePoint);
            return passiveSkillsData;
        }

        private SkillExpenditureRequest GetSkillExpenditureRequest(string playerId)
        {
            if (_playerToSkillExpenditureRequest.ContainsKey(playerId))
            {
                return _playerToSkillExpenditureRequest[playerId];
            }

            var newSkillExpenditureRequest = new SkillExpenditureRequest(playerId);
            _playerToSkillExpenditureRequest.Add(playerId, newSkillExpenditureRequest);
            return newSkillExpenditureRequest;
        }

        private void ResetSkillExpenditureRequest(string playerId)
        {
            _playerToSkillExpenditureRequest[playerId] = new SkillExpenditureRequest(playerId);
        }


        private async void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            _passiveSkillsData = await InitializePassiveSkillsData();
            _activeSkillsData = await InitializeActiveSkillsData();

            //--if there is an active game then load that, otherwise prompt to start a new game
            if (_settingsDataContainer.Values.TryGetValue(ActiveGameIdSetting, out var activeGameId))
            {
                try
                {
                    _gameModel = await _fungusToastApiClient.GetGameState(int.Parse(activeGameId.ToString()));
                    _usersPlayingLocalGame = _gameModel.Players.Where(player => _validUserNames.Contains(player.Name))
                        .Select(player => player.Name).ToList();

                    InitializeGame(_gameModel);
                }
                catch (GameNotFoundException gameNotFoundException)
                {
                    //--this should only happen if the database was reset while a game was in progress. This happens a lot while the game is actively being developed.
                    Debug.WriteLine(gameNotFoundException);
                    ClearGame();
                    await GameSettingsDialog.ShowAsync();
                }
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
                _usersPlayingLocalGame.Add(_validUserNames[0]);
                var newGameRequest = new NewGameRequest(_validUserNames[0], numberOfHumanPlayers, numberOfAiPlayers);
                _gameModel = await _fungusToastApiClient.CreateGame(newGameRequest);
                _settingsDataContainer.Values[ActiveGameIdSetting] = _gameModel.Id;

                JoinGameResult joinGameResult = null;
                for (int i = 1; i < numberOfHumanPlayers; i++)
                {
                    _usersPlayingLocalGame.Add(_validUserNames[i]);
                    var joinGameRequest = new JoinGameRequest(_gameModel.Id, _validUserNames[i]);
                    joinGameResult = await _fungusToastApiClient.JoinGame(joinGameRequest);
                }

                var readyToPlay = _gameModel.Status == "Started" ||
                                  (joinGameResult != null && joinGameResult.ResultTypeEnumValue ==
                                   JoinGameResult.JoinGameResultType.JoinedStarted);

                if (readyToPlay)
                {
                    _gameModel = await _fungusToastApiClient.GetGameState(_gameModel.Id);
                   InitializeGame(_gameModel);
                }
                else
                {
                    //TODO will add support for creating a game and waiting for another player to join eventually
                }
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
                .OrderByDescending(player => player.Human)
                .ThenBy(player => player.Id)
                .ToList();

            //--get the number of action points earned for each player and reduce the player's starting action points by this amount so we can render updates
            //  back up to this number. Since mutation points always start at 0 each round, we don't have this same complication.
            var playerIdToStartingActionPoints = reorderedPlayers.ToDictionary(x => x.Id, y => y.ActionPoints);
            foreach (var growthCycle in game.GrowthCycles)
            {
                foreach (var keyValuePair in growthCycle.ActionPointsEarned)
                {
                    playerIdToStartingActionPoints[keyValuePair.Key] -= keyValuePair.Value;
                }
            }

            for (var i = 1; i <= reorderedPlayers.Count; i++)
            {
                var playerState = reorderedPlayers[i - 1];
                var player = new Player(playerState.Name, _availableColors[i - 1], playerState.Id,
                    playerState.Human, _passiveSkillsData, _activeSkillsData);

                player.ActionPoints = playerIdToStartingActionPoints[player.PlayerId];

                UpdatePlayer(player, playerState, game.StartingPlayerStats);
                players.Add(player);
            }

            ViewModel.Players = players;

            foreach (var player in players)
            {
                _playerNumberToColorBrushDictionary.Add(player.PlayerId, new SolidColorBrush(player.Color));
                _playerNumberToMutationButtons.Add(player.PlayerId, new Dictionary<string, Button>());
                _playerNumberToActiveSkillsButtons.Add(player.PlayerId, new Dictionary<string, Button>());
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

            var gridLinesColorBrush = new SolidColorBrush(Colors.Gray);
            var noPaddingOrMargin = new Thickness(0);
            var previousGameState = game.StartingGameState;

            for (var i = 0; i < game.NumberOfCells; i++)
            {
                Brush backgroundBrush;
                char gridCellContent = ' ';
                if (previousGameState.CellsDictionary.ContainsKey(i))
                {
                    var cell = previousGameState.CellsDictionary[i];

                    if (cell.Moist)
                    {
                        backgroundBrush = _moistCellBrush;
                    }
                    else if (!cell.Live)
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

                var cellButton = new Button
                {
                    Style = Resources["ButtonRevealStyle"] as Style,
                    Background = backgroundBrush,
                    Content = gridCellContent,
                    BorderBrush = gridLinesColorBrush,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    FontStretch = FontStretch.UltraCondensed,
                    Margin = noPaddingOrMargin,
                    Padding = noPaddingOrMargin,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 10,
                    Tag = i //--set the tag to the grid cell number
                };

                cellButton.Click += GridCellOnClick;

                Toast.Children.Add(cellButton);
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

                //--since players must always spend all mutation points each round, we can assume we are starting at 0 and render all of the growth cycle mutation points
                foreach (var mutationPointEarned in growthCycle.MutationPointsEarned)
                {
                    tasks.Add(RenderMutationPointEarned(mutationPointEarned));
                }

                foreach (var actionPointEarned in growthCycle.ActionPointsEarned)
                {
                    tasks.Add(RenderActionPointEarned(actionPointEarned));
                }

                //--attempt to render this prior to the next await so the UI catches the change
                RenderPlayerStatsChanges(growthCycle.PlayerStatsChanges);

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

        private async Task RenderToastChange(ToastChange toastChange)
        {
            var currentGridCell = Toast.Children[toastChange.Index] as Button;
            currentGridCell.Opacity = 0;

            if (toastChange.Live)
            {
                currentGridCell.Background = _playerNumberToColorBrushDictionary[toastChange.PlayerId];
                currentGridCell.Content = string.Empty;
            }
            else if (toastChange.Moist)
            {
                currentGridCell.Background = _moistCellBrush;
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
                mutationPointAnnouncementMessage.Text = $"+{mutationPointEarned.Value}!";
                ViewModel.Players.First(player => player.PlayerId == mutationPointEarned.Key).AvailableMutationPoints +=
                    mutationPointEarned.Value;

                mutationPointAnnouncementMessage.Opacity = 1;
                await mutationPointAnnouncementMessage.Fade(0, 1500).StartAsync();
            }
        }

        private async Task RenderActionPointEarned(KeyValuePair<string, int> actionPointEarned)
        {
            if (actionPointEarned.Value > 0)
            {
                var actionPointAnnouncementMessage =
                    _playerNumberToActionPointAnnouncementTextBlock[actionPointEarned.Key];
                actionPointAnnouncementMessage.Text = $"+{actionPointEarned.Value}!";
                ViewModel.GetPlayer(actionPointEarned.Key).ActionPoints +=
                    actionPointEarned.Value;

                actionPointAnnouncementMessage.Opacity = 1;
                await actionPointAnnouncementMessage.Fade(0, 1500).StartAsync();
            }
        }

        private void RenderPlayerStatsChanges(Dictionary<string, PlayerStatsChanges> playerStatsChanges)
        {
            foreach (var keyValuePair in playerStatsChanges)
            {
                var playerId = keyValuePair.Key;
                var playerStatsChange = keyValuePair.Value;

                var player = ViewModel.GetPlayer(playerId);
                player.PerishedCells += playerStatsChange.PerishedCells;
                player.GrownCells += playerStatsChange.GrownCells;
                player.RegeneratedCells += playerStatsChange.RegeneratedCells;
                player.LostDeadCells += playerStatsChange.LostDeadCells;
                player.StolenDeadCells += playerStatsChange.StolenDeadCells;
                player.FungicidalKills += playerStatsChange.FungicidalKills;

                player.LiveCells = player.LiveCells + playerStatsChange.GrownCells +
                                   playerStatsChange.RegeneratedCells + playerStatsChange.StolenDeadCells -
                                   playerStatsChange.PerishedCells;

                player.DeadCells += playerStatsChange.PerishedCells - playerStatsChange.RegeneratedCells -
                                    playerStatsChange.LostDeadCells;
            }
        }

        private void SetGameStats(GameModel game)
        {
            ViewModel.RoundNumber = game.RoundNumber;
            ViewModel.GenerationNumber = game.GenerationNumber;
            ViewModel.TotalDeadCells = game.TotalDeadCells;
            ViewModel.TotalEmptyCells = game.TotalEmptyCells;
            ViewModel.TotalLiveCells = game.TotalLiveCells;
            ViewModel.TotalMoistCells = game.TotalMoistCells;
            ViewModel.TotalRegeneratedCells = game.TotalRegeneratedCells;
            ViewModel.LightLevel = game.LightLevel;
            if (game.EndOfGameCountDown.HasValue)
            {
                ViewModel.GameEndCountDown = game.EndOfGameCountDown.Value;
                GameEndNotKnownTextBlock.Visibility = Visibility.Collapsed;
                EndOfGameCountDownTextBlock.Visibility = Visibility.Visible;
            }
        }

        private void UpdatePlayer(IPlayer playerToUpdate, PlayerState playerStateValuesToCopy,
            Dictionary<string, PlayerStats> startingPlayerStats = null)
        {
            //--zero out AI players' mutation points since they always spend them immediately
            if (!playerToUpdate.IsHuman)
            {
                playerToUpdate.AvailableMutationPoints = 0;
            }

            if (startingPlayerStats == null || startingPlayerStats.Count == 0)
            {
                if (playerToUpdate.DeadCells != playerStateValuesToCopy.DeadCells)
                {
                    Debug.WriteLine($"DeadCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.DeadCells}, right side is {playerStateValuesToCopy.DeadCells}");
                }

                if (playerToUpdate.LiveCells != playerStateValuesToCopy.LiveCells)
                {
                    Debug.WriteLine($"LiveCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.LiveCells}, right side is {playerStateValuesToCopy.LiveCells}");
                }

                if (playerToUpdate.RegeneratedCells != playerStateValuesToCopy.RegeneratedCells)
                {
                    Debug.WriteLine($"RegeneratedCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.RegeneratedCells}, right side is {playerStateValuesToCopy.RegeneratedCells}");
                }

                if (playerToUpdate.LostDeadCells != playerStateValuesToCopy.LostDeadCells)
                {
                    Debug.WriteLine($"LostDeadCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.LostDeadCells}, right side is {playerStateValuesToCopy.LostDeadCells}");
                }

                if (playerToUpdate.StolenDeadCells != playerStateValuesToCopy.StolenDeadCells)
                {
                    Debug.WriteLine($"StolenDeadCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.StolenDeadCells}, right side is {playerStateValuesToCopy.StolenDeadCells}");
                }

                if (playerToUpdate.GrownCells != playerStateValuesToCopy.GrownCells)
                {
                    Debug.WriteLine($"GrownCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.GrownCells}, right side is {playerStateValuesToCopy.GrownCells}");
                }

                if (playerToUpdate.PerishedCells != playerStateValuesToCopy.PerishedCells)
                {
                    Debug.WriteLine($"PerishedCells Player stats got out of sync with toast changes! Left side is {playerToUpdate.PerishedCells}, right side is {playerStateValuesToCopy.PerishedCells}");
                }

                if(playerToUpdate.FungicidalKills != playerStateValuesToCopy.FungicidalKills)
                {
                    Debug.WriteLine($"FungicidalKills Player stats got out of sync with toast changes! Left side is {playerToUpdate.FungicidalKills}, right side is {playerStateValuesToCopy.FungicidalKills}");
                }
                playerToUpdate.DeadCells = playerStateValuesToCopy.DeadCells;
                playerToUpdate.LiveCells = playerStateValuesToCopy.LiveCells;
                playerToUpdate.RegeneratedCells = playerStateValuesToCopy.RegeneratedCells;
                playerToUpdate.LostDeadCells = playerStateValuesToCopy.LostDeadCells;
                playerToUpdate.StolenDeadCells = playerStateValuesToCopy.StolenDeadCells;
                playerToUpdate.GrownCells = playerStateValuesToCopy.GrownCells;
                playerToUpdate.PerishedCells = playerStateValuesToCopy.PerishedCells;
                playerToUpdate.FungicidalKills = playerStateValuesToCopy.FungicidalKills;
            }
            else
            {
                var playerStats = startingPlayerStats[playerToUpdate.PlayerId];
                playerToUpdate.DeadCells = playerStats.DeadCells;
                playerToUpdate.LiveCells = playerStats.LiveCells;
                playerToUpdate.RegeneratedCells = playerStats.RegeneratedCells;
                playerToUpdate.LostDeadCells = playerStats.LostDeadCells;
                playerToUpdate.StolenDeadCells = playerStats.StolenDeadCells;
                playerToUpdate.GrownCells = playerStats.GrownCells;
                playerToUpdate.PerishedCells = playerStats.PerishedCells;
                playerToUpdate.FungicidalKills = playerStats.FungicidalKills;
            }

            playerToUpdate.SpentMutationPoints = playerStateValuesToCopy.SpentMutationPoints;

            playerToUpdate.HyperMutationSkillLevel = playerStateValuesToCopy.HyperMutationSkillLevel;
            playerToUpdate.AntiApoptosisSkillLevel = playerStateValuesToCopy.AntiApoptosisSkillLevel;
            playerToUpdate.RegenerationSkillLevel = playerStateValuesToCopy.RegenerationSkillLevel;
            playerToUpdate.BuddingSkillLevel = playerStateValuesToCopy.BuddingSkillLevel;
            playerToUpdate.MycotoxinsSkillLevel = playerStateValuesToCopy.MycotoxinsSkillLevel;
            playerToUpdate.HydrophiliaSkillLevel = playerStateValuesToCopy.HydrophiliaSkillLevel;
            playerToUpdate.SporesSkillLevel = playerStateValuesToCopy.SporesSkillLevel;

            var updatedGrowthScorecard = new GrowthScorecard
            {
                ApoptosisChancePercentage = playerStateValuesToCopy.ApoptosisChance,
                StarvedCellDeathChancePercentage = playerStateValuesToCopy.StarvedCellDeathChance,
                MutationChancePercentage = playerStateValuesToCopy.MutationChance,
                RegenerationChancePercentage = playerStateValuesToCopy.RegenerationChance,
                MycotoxinFungicideChancePercentage = playerStateValuesToCopy.MycotoxinFungicideChance,
                MoistureGrowthBoost = playerStateValuesToCopy.MoistureGrowthBoost,
                SporesChancePercentage = playerStateValuesToCopy.SporesChance,
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

            UpdateActiveSkillBorders(playerToUpdate);
        }

        private void UpdateActiveSkillBorders(IPlayer playerToUpdate)
        {
            //--make sure there are values loaded into the dictionary (due to timing issue when the game is loaded and already in progress
            if (playerToUpdate.IsLocalPlayer(_usersPlayingLocalGame) 
                && _playerNumberToPassiveSkillsBorder.Count > 0
                && _playerNumberToActiveSkillsBorder.Count > 0)
            {
                var passiveSkillsBorder = _playerNumberToPassiveSkillsBorder[playerToUpdate.PlayerId];
                if (playerToUpdate.AvailableMutationPoints > 0)
                {
                    passiveSkillsBorder.BorderBrush = _activeBorderBrush;
                    passiveSkillsBorder.BorderThickness = _activeThickness;
                }
                else
                {
                    passiveSkillsBorder.BorderBrush = _normalBorderBrush;
                    passiveSkillsBorder.BorderThickness = _normalThickness;
                }

                var activeSkillsBorder = _playerNumberToActiveSkillsBorder[playerToUpdate.PlayerId];
                if (playerToUpdate.ActionPoints > 0)
                {
                    activeSkillsBorder.BorderBrush = _activeBorderBrush;
                    activeSkillsBorder.BorderThickness = _activeThickness;
                }
                else
                {
                    activeSkillsBorder.BorderBrush = _normalBorderBrush;
                    activeSkillsBorder.BorderThickness = _normalThickness;
                }
            }
        }

        private void EnableMutationButtons(IPlayer player)
        {
            var buttons = _playerNumberToMutationButtons[player.PlayerId];
            foreach (var mutationButton in buttons)
            {
                EnableMutationButtonIfAppropriate(player, mutationButton.Value);
            }
        }

        private void EnableMutationButtonIfAppropriate(IPlayer player, Button button)
        {
            if (player.AvailableMutationPoints == 0)
            {
                button.IsEnabled = false;
            }else if (button.Name == "AntiApoptosisButton" && player.GrowthScorecard.ApoptosisChancePercentage <= 0)
            {
                button.IsEnabled = false;
            }
            else
            {
                button.IsEnabled = true;
            }
        }

        private void UpdateActiveSkillButtons(IPlayer player)
        {
            var buttons = _playerNumberToActiveSkillsButtons[player.PlayerId];
            foreach (var activeSkillButton in buttons)
            {
                EnableActiveSkillButtonIfAppropriate(player, activeSkillButton.Value);
            }
        }

        private void EnableActiveSkillButtonIfAppropriate(IPlayer player, Button button)
        {
            button.IsEnabled = player.ActionPoints > 0
                               && ActiveSkillShouldBeEnabled(button.Name);
        }

        private bool ActiveSkillShouldBeEnabled(string buttonName)
        {
            int activeSkillId = _activeSkillButtonNameToActiveSkillId[buttonName];
            return ViewModel.TotalEmptyCells >= _activeSkillsData.GetNumberOfActionsPerActionPoint(activeSkillId)
                   && ViewModel.RoundNumber >= _activeSkillsData.GetMinimumRound(activeSkillId);
        }

        private async Task CheckForRemainingMutationPoints(IPlayer player)
        {
            var hasMutationPointsLeft = player.AvailableMutationPoints > 0;
            if (!hasMutationPointsLeft)
            {
                DisablePlayerMutationButtons(player);
            }

            if (hasMutationPointsLeft)
            {
                return;
            }

            //--disable skill trees when spending points in case we render updates. Spending points -- especially on active skills -- could run into race conditions if the toast is updating
            DisableSkillTrees();

            var skillUpdateResult = await _fungusToastApiClient.PushSkillExpenditures(_gameModel.Id, player.PlayerId, GetSkillExpenditureRequest(player.PlayerId));

            ResetSkillExpenditureRequest(player.PlayerId);

            if (skillUpdateResult.NextRoundAvailable)
            {
                _gameModel = await _fungusToastApiClient.GetGameState(_gameModel.Id);

                await RenderUpdates(_gameModel);

                await CheckForGameEnd();
                
                EnableMutationButtons(player);
                UpdateActiveSkillButtons(player);
            }

            EnableSkillTrees();
        }

        private async void IncreaseHypermutation_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;

            player.IncreaseHypermutation();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseHypermutation();

            await CheckForRemainingMutationPoints(player);
        }

        private async void AntiApoptosis_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.DecreaseApoptosisChance();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseAntiApoptosis();
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
            GetSkillExpenditureRequest(player.PlayerId).IncreaseBudding();

            await CheckForRemainingMutationPoints(player);
        }

        private async void IncreaseRegeneration_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseRegeneration();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseRegeneration();

            await CheckForRemainingMutationPoints(player);
        }

        private async void MycotoxinFungicideButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseMycotoxicity();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseMycotoxicity();

            await CheckForRemainingMutationPoints(player);
        }
        private async void SporesButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseSpores();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseSpores();

            await CheckForRemainingMutationPoints(player);
        }

        private async void Hydrophilia_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.IncreaseHydrophilia();
            GetSkillExpenditureRequest(player.PlayerId).IncreaseHydrophilia();

            await CheckForRemainingMutationPoints(player);
        }

        private void EyeDropper_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.UseEyeDropper();
            GetSkillExpenditureRequest(player.PlayerId).UseEyeDropper();

            EnableActiveSkill(player, ActiveSkills.EyeDropper, _activeSkillsData.GetNumberOfActionsPerActionPoint((int)ActiveSkills.EyeDropper));
        }

        private void DeadCell_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.UseDeadCell();
            GetSkillExpenditureRequest(player.PlayerId).UseDeadCell();

            EnableActiveSkill(player, ActiveSkills.DeadCell, _activeSkillsData.GetNumberOfActionsPerActionPoint((int)ActiveSkills.DeadCell));
        }

        private void IncreaseLighting_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.UseIncreaseLighting();
            GetSkillExpenditureRequest(player.PlayerId).UseIncreaseLighting();
            ViewModel.LightLevel++;

            if (ViewModel.ActiveCellChangesRemaining == 0)
            {
                UpdateActiveSkillButtons(player);
            }
        }

        private void DecreaseLighting_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            player.UseDecreaseLighting();
            GetSkillExpenditureRequest(player.PlayerId).UseDecreaseLighting();
            ViewModel.LightLevel--;

            if (ViewModel.ActiveCellChangesRemaining == 0)
            {
                UpdateActiveSkillButtons(player);
            }
        }

        private void EnableActiveSkill(IPlayer activePlayer, ActiveSkills activeSkill, int numberOfActions)
        {
            DisableSkillTrees();

            ViewModel.ActivePlayerId = activePlayer.PlayerId;
            ViewModel.ActiveSkill = activeSkill;
            ViewModel.ActiveCellChangesRemaining = numberOfActions;

            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Person, 1);
        }

        private void DisableSkillTrees()
        {
            foreach (var player in ViewModel.Players)
            {
                DisablePlayerMutationButtons(player);
                DisablePlayerActionButtons(player);
                _playerNumberToPassiveSkillTreeDialogButton[player.PlayerId].IsEnabled = false;
                _playerNumberToActiveSkillsDialogButton[player.PlayerId].IsEnabled = false;

                var passiveSkillsBorder = _playerNumberToPassiveSkillsBorder[player.PlayerId];
                passiveSkillsBorder.BorderBrush = _normalBorderBrush;
                passiveSkillsBorder.BorderThickness = _normalThickness;

                var activeSkillsBorder = _playerNumberToActiveSkillsBorder[player.PlayerId];
                activeSkillsBorder.BorderBrush = _normalBorderBrush;
                activeSkillsBorder.BorderThickness = _normalThickness;
            }
        }

        private void EnableSkillTrees()
        {
            foreach (var player in ViewModel.Players)
            {
                EnableMutationButtons(player);
                UpdateActiveSkillButtons(player);
                var skillTreeButton = _playerNumberToPassiveSkillTreeDialogButton[player.PlayerId];
                var activeSkillButton = _playerNumberToActiveSkillsDialogButton[player.PlayerId];
                if (player.IsLocalPlayer(_usersPlayingLocalGame))
                {
                    if (player.AvailableMutationPoints > 0)
                    {
                        var passiveSkillsBorder = _playerNumberToPassiveSkillsBorder[player.PlayerId];
                        passiveSkillsBorder.BorderBrush = _activeBorderBrush;
                        passiveSkillsBorder.BorderThickness = _activeThickness;
                    }

                    if (player.ActionPoints > 0)
                    {
                        var activeSkillsBorder = _playerNumberToActiveSkillsBorder[player.PlayerId];
                        activeSkillsBorder.BorderBrush = _activeBorderBrush;
                        activeSkillsBorder.BorderThickness = _activeThickness;
                    }
                }
                
                skillTreeButton.IsEnabled = true;
                activeSkillButton.IsEnabled = true;
            }
        }

        private void GridCellOnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var gridCellIndex = int.Parse(button.Tag.ToString());

            //--as of 2019-06-22, the only active skills are Eye Dropper and Dead Cell, and these both require an empty space to click on
            if (ViewModel.ActivePlayerId == null || button.Background != _emptyCellBrush || button.Background == _moistCellBrush) return;

            ViewModel.ActiveCellChangesRemaining--;
            var toolTip = new ToolTip();

            switch (ViewModel.ActiveSkill.Value)
            {
                case ActiveSkills.EyeDropper:
                    GetSkillExpenditureRequest(ViewModel.ActivePlayerId).AddMoistureDroplet(gridCellIndex);
                    toolTip.Content = "This cell is moist. See the Hydrophilia skill.";
                    button.Background = _moistCellBrush;
                    ViewModel.TotalMoistCells++;
                    ViewModel.TotalEmptyCells--;
                    break;
                case ActiveSkills.DeadCell:
                    GetSkillExpenditureRequest(ViewModel.ActivePlayerId).AddDeadCell(gridCellIndex);
                    toolTip.Content = "This dead cell was placed by the Dead Cell active skill.";
                    button.Background = _deadCellBrush;
                    button.Content = _deadCellSymbol;
                    ViewModel.TotalDeadCells++;
                    ViewModel.TotalEmptyCells--;
                    break;
                default:
                    throw new Exception(
                        $"There is an active player but the active skill with id {ViewModel.ActiveSkill.Value} is not valid.");
            }

            ToolTipService.SetToolTip(button, toolTip);
            if (ViewModel.ActiveCellChangesRemaining == 0)
            {
                EnableSkillTrees();
                Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 2);
                var player = ViewModel.GetPlayer(ViewModel.ActivePlayerId);
                ViewModel.ActivePlayerId = null;
                ViewModel.ActiveSkill = null;
                UpdateActiveSkillButtons(player);
            }
        }

        private void DisablePlayerMutationButtons(IPlayer player)
        {
            var playerMutationButtons = _playerNumberToMutationButtons[player.PlayerId];

            foreach (var button in playerMutationButtons)
            {
                button.Value.IsEnabled = false;
            }

            ClosePassiveSkillsDialog(player);
        }

        private void DisablePlayerActionButtons(IPlayer player)
        {
            var buttons = _playerNumberToActiveSkillsButtons[player.PlayerId];

            foreach (var button in buttons)
            {
                button.Value.IsEnabled = false;
            }

            CloseActiveSkillsDialog(player);
        }

        private void CloseActiveSkillsDialog(IPlayer player)
        {
            var activeSkillsDialog = _playerNumberToActiveSkillsDialog[player.PlayerId];
            activeSkillsDialog.Hide();
        }

        private void ClosePassiveSkillsDialog(IPlayer player)
        {
            var skillTreeDialog = _playerNumberToSkillTreeDialog[player.PlayerId];
            skillTreeDialog.Hide();
        }

        private void MutationOptionButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var playerButtons = _playerNumberToMutationButtons[player.PlayerId];

            if (player.AvailableMutationPoints > 0 && player.IsLocalPlayer(_usersPlayingLocalGame))
            {
                EnableMutationButtonIfAppropriate(player, button);
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
            var playerId = mutationPointMessageTextBlock.Tag.ToString();
            _playerNumberToMutationPointAnnouncementTextBlock[playerId] = mutationPointMessageTextBlock;
        }

        private void ActionPointMessage_Loaded(object sender, RoutedEventArgs e)
        {
            var textBlock = sender as TextBlock;
            var playerId = textBlock.Tag.ToString();
            _playerNumberToActionPointAnnouncementTextBlock[playerId] = textBlock;
        }

        private void SkillsBorder_Loaded(object sender, RoutedEventArgs e)
        {
            var skillsBorder = sender as Border;
            var playerId = skillsBorder.Tag.ToString();
            _playerNumberToPassiveSkillsBorder[playerId] = skillsBorder;
        }

        private async void ActiveSkillsBorder_Loaded(object sender, RoutedEventArgs e)
        {
            var activeSkillsBorder = sender as Border;
            var playerId = activeSkillsBorder.Tag.ToString();
            _playerNumberToActiveSkillsBorder[playerId] = activeSkillsBorder;
            if (_gameModel != null && _playerNumberToActiveSkillsBorder.Keys.Count == _gameModel.Players.Count)
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

            if (player.IsLocalPlayer(_usersPlayingLocalGame))
            {
                EnableMutationButtons(player);
            }
        }

        private void Dialog_OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            _visibleDialog = null;
        }

        private async void PassiveSkillTreeDialogButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var contentDialog = _playerNumberToSkillTreeDialog[player.PlayerId];
            _visibleDialog = contentDialog;
            await contentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void PassiveSkillTreeDialogButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var playerId = button.Tag.ToString();
            _playerNumberToPassiveSkillTreeDialogButton[playerId] = button;
        }

        private void ActiveSkillTreeDialogButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var playerId = button.Tag.ToString();
            _playerNumberToActiveSkillsDialogButton[playerId] = button;
        }

        private void ActiveSkillButton_Loaded(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var playerId = button.Tag.ToString();
            var player = ViewModel.GetPlayer(playerId);

            if (player.ActionPoints > 0 && player.IsLocalPlayer(_usersPlayingLocalGame))
            {
                EnableActiveSkillButtonIfAppropriate(player, button);
            }

            var playerButtons = _playerNumberToActiveSkillsButtons[player.PlayerId];
            //--make sure the buttons are only added to the dictionary once
            if (!playerButtons.ContainsKey(button.Name))
            {
                playerButtons.Add(button.Name, button);
            }
        }

        private async void ActiveSkillsButton_OnClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var player = button.DataContext as IPlayer;
            var contentDialog = _playerNumberToActiveSkillsDialog[player.PlayerId];
            _visibleDialog = contentDialog;
            await contentDialog.ShowAsync(ContentDialogPlacement.Popup);
        }

        private void ActiveSkillsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var dialog = sender as ContentDialog;
            var playerId = dialog.Tag.ToString();
            _playerNumberToActiveSkillsDialog[playerId] = dialog;
        }

        private void ActiveSkillsDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            var skillTreeDialog = sender as ContentDialog;
            var player = skillTreeDialog.DataContext as IPlayer;

            if (player.IsLocalPlayer(_usersPlayingLocalGame))
            {
                UpdateActiveSkillButtons(player);
            }
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

        private readonly Dictionary<string, int> _activeSkillButtonNameToActiveSkillId = new Dictionary<string, int>
        {
            {"EyeDropper", (int)ActiveSkills.EyeDropper},
            {"DeadCell", (int)ActiveSkills.DeadCell},
            {"IncreaseLighting", (int)ActiveSkills.IncreaseLight},
            {"DecreaseLighting", (int)ActiveSkills.DecreaseLight}
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
