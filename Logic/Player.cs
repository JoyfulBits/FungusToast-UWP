﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Logic.Annotations;

namespace Logic
{
    public class Player : IPlayer
    {
        private readonly PassiveSkillsData _passiveSkillsData;
        private readonly ActiveSkillsData _activeSkillsData;
        private Color _color;
        private string _playerId;
        private GrowthScorecard _growthScorecard;
        private int _liveCells;
        private int _deadCells;

        private string _name;
        private int _regeneratedCells;
        private int _availableMutationPoints;
        private float _hyperMutationSkillLevel;
        private float _antiApoptosisSkillLevel;
        private float _regenerationSkillLevel;
        private float _buddingSkillLevel;
        private float _mycotoxinsSkillLevel;
        private int _grownCells;
        private int _perishedCells;
        private int _fungicidalKills;
        private int _spentMutationPoints;
        private float _hydrophiliaSkillLevel;
        private int _lostDeadCells;
        private int _stolenDeadCells;
        private float _sporesSkillLevel;
        private int _actionPoints;

        public Player(string name, Color playerCellColor, string playerId,
            bool isHuman, PassiveSkillsData passiveSkillsData, ActiveSkillsData activeSkillsData)
        {
            _passiveSkillsData = passiveSkillsData;
            this._activeSkillsData = activeSkillsData;
            Name = name;
            Color = playerCellColor;
            PlayerId = playerId;
            IsHuman = isHuman;
            _growthScorecard = new GrowthScorecard();
        }

        public bool IsHuman { get; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public int AvailableMutationPoints
        {
            get => _availableMutationPoints;
            set
            {
                if (value == _availableMutationPoints) return;
                _availableMutationPoints = value;
                OnPropertyChanged();
            }
        }

        public int ActionPoints
        {
            get => _actionPoints;
            set
            {
                if (value == _actionPoints) return;
                _actionPoints = value;
                OnPropertyChanged();
            }
        }

        public Color Color
        {
            get => _color;
            set
            {
                if (value.Equals(_color)) return;
                _color = value;
                OnPropertyChanged();
            }
        }

        public string PlayerId
        {
            get => _playerId;
            set
            {
                if (value == _playerId) return;
                _playerId = value;
                OnPropertyChanged();
            }
        }

        public GrowthScorecard GrowthScorecard
        {
            get => _growthScorecard;
            set
            {
                if (Equals(value, _growthScorecard)) return;
                _growthScorecard = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TopLeftGrowthChance));
                OnPropertyChanged(nameof(TopGrowthChance));
                OnPropertyChanged(nameof(TopRightGrowthChance));
                OnPropertyChanged(nameof(RightGrowthChance));
                OnPropertyChanged(nameof(BottomRightGrowthChance));
                OnPropertyChanged(nameof(BottomGrowthChance));
                OnPropertyChanged(nameof(BottomLeftGrowthChance));
                OnPropertyChanged(nameof(LeftGrowthChance));

                OnPropertyChanged(nameof(ApoptosisChancePercentage));
                OnPropertyChanged(nameof(StarvedCellDeathChancePercentage));
                OnPropertyChanged(nameof(RegenerationChancePercentage));
                OnPropertyChanged(nameof(MutationChancePercentage));
                OnPropertyChanged(nameof(MycotoxinFungicideChancePercentage));
            }
        }

        public float ApoptosisChancePercentage => GrowthScorecard.ApoptosisChancePercentage;
        public float StarvedCellDeathChancePercentage => GrowthScorecard.StarvedCellDeathChancePercentage;
        public float RegenerationChancePercentage => GrowthScorecard.RegenerationChancePercentage;
        public float MutationChancePercentage => GrowthScorecard.MutationChancePercentage;
        public float MycotoxinFungicideChancePercentage => GrowthScorecard.MycotoxinFungicideChancePercentage;


        public string TopLeftGrowthChance =>
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft].ToString("0.00\\%");
        public string TopGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Top].ToString("0.00\\%");
        public string TopRightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopRight].ToString("0.00\\%");
        public string RightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Right].ToString("0.00\\%");
        public string BottomRightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomRight].ToString("0.00\\%");
        public string BottomGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Bottom].ToString("0.00\\%");
        public string BottomLeftGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomLeft].ToString("0.00\\%");
        public string LeftGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Left].ToString("0.00\\%");

        public float HyperMutationSkillLevel
        {
            get => _hyperMutationSkillLevel;
            set
            {
                if (value.Equals(_hyperMutationSkillLevel)) return;
                _hyperMutationSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float AntiApoptosisSkillLevel
        {
            get => _antiApoptosisSkillLevel;
            set
            {
                if (value.Equals(_antiApoptosisSkillLevel)) return;
                _antiApoptosisSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float RegenerationSkillLevel
        {
            get => _regenerationSkillLevel;
            set
            {
                if (value.Equals(_regenerationSkillLevel)) return;
                _regenerationSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float BuddingSkillLevel
        {
            get => _buddingSkillLevel;
            set
            {
                if (value.Equals(_buddingSkillLevel)) return;
                _buddingSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float MycotoxinsSkillLevel
        {
            get => _mycotoxinsSkillLevel;
            set
            {
                if (value.Equals(_mycotoxinsSkillLevel)) return;
                _mycotoxinsSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float HydrophiliaSkillLevel
        {
            get => _hydrophiliaSkillLevel;
            set
            {
                if (value.Equals(_hydrophiliaSkillLevel)) return;
                _hydrophiliaSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public float SporesSkillLevel
        {
            get => _sporesSkillLevel;
            set
            {
                if (value.Equals(_sporesSkillLevel)) return;
                _sporesSkillLevel = value;
                OnPropertyChanged();
            }
        }

        public bool HasPointsToSpend => AvailableMutationPoints > 0 || ActionPoints > 0;

        public int LiveCells
        {
            get => _liveCells;
            set
            {
                if (value == _liveCells) return;
                _liveCells = value;
                OnPropertyChanged();
            }
        }

        public int DeadCells
        {
            get => _deadCells;
            set
            {
                if (value == _deadCells) return;
                _deadCells = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Represents the number of regenerated and stolen dead cells (which are really just other player's dead cells that were regenerated)
        /// </summary>
        public int TotalRegrownCells => _regeneratedCells + _stolenDeadCells;

        public int RegeneratedCells
        {
            get => _regeneratedCells;
            set
            {
                if (value == _regeneratedCells) return;
                _regeneratedCells = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalRegrownCells));
            }
        }

        public int LostDeadCells
        {
            get => _lostDeadCells;
            set
            {
                if (value == _lostDeadCells) return;
                _lostDeadCells = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DeadCells));
            }
        }

        public int StolenDeadCells
        {
            get => _stolenDeadCells;
            set
            {
                if (value == _stolenDeadCells) return;
                _stolenDeadCells = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalRegrownCells));
            }
        }

        public int GrownCells
        {
            get => _grownCells;
            set
            {
                if (value == _grownCells) return;
                _grownCells = value;
                OnPropertyChanged();
            }
        }

        public int PerishedCells
        {
            get => _perishedCells;
            set
            {
                if (value == _perishedCells) return;
                _perishedCells = value;
                OnPropertyChanged();
            }
        }

        public int FungicidalKills
        {
            get => _fungicidalKills;
            set
            {
                if (value == _fungicidalKills) return;
                _fungicidalKills = value;
                OnPropertyChanged();
            }
        }

        public int SpentMutationPoints
        {
            get => _spentMutationPoints;
            set
            {
                if (value == _spentMutationPoints) return;
                _spentMutationPoints = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string AddMutationChanceMessage => _passiveSkillsData.IncreaseMutationChanceMessage;
        public string AddCornerGrowthChanceMessage => _passiveSkillsData.IncreaseCornerGrowthChanceMessage;
        public string DecreaseApoptosisChanceMessage => _passiveSkillsData.DecreaseApoptosisChanceMessage;
        public string AddRegrowthChanceMessage => _passiveSkillsData.IncreaseRegrowthChanceMessage;
        public string AddMycotoxicityChanceMessage => _passiveSkillsData.IncreaseMycotoxinFungicideChanceMessage;
        public string AddMoistureGrowthBoostMessage => _passiveSkillsData.IncreaseMoistureGrowthBoostMessage;
        public string IncreaseSporesChanceMessage => _passiveSkillsData.IncreaseSporesChanceMessage;
        //TODO need to pull out the ActiveSkills class into a common place that can be referenced from both Logic and ApiClient
        public string AddWaterDropletMessage => _activeSkillsData.GetActiveSkillMessage(1);
        //TODO need to pull out the ActiveSkills class into a common place that can be referenced from both Logic and ApiClient
        public string AddDeadCellMessage => _activeSkillsData.GetActiveSkillMessage(2);
        public string IncreaseLightingMessage => _activeSkillsData.GetActiveSkillMessage(3);
        public string DecreaseLightingMessage => _activeSkillsData.GetActiveSkillMessage(4);

        public void IncreaseHypermutation()
        {
            GrowthScorecard.MutationChancePercentage += _passiveSkillsData.MutationPercentageChancePerAttributePoint;
            AvailableMutationPoints--;
        }

        public void DecreaseApoptosisChance()
        {
            GrowthScorecard.ApoptosisChancePercentage -= _passiveSkillsData.ReducedApoptosisPercentagePerAttributePoint;
            AvailableMutationPoints--;
        }

        public void IncreaseBudding()
        {
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft] +=
                _passiveSkillsData.CornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopRight] +=
                _passiveSkillsData.CornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomRight] +=
                _passiveSkillsData.CornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomLeft] +=
                _passiveSkillsData.CornerGrowthChancePerAttributePoint;

            AvailableMutationPoints--;

            OnPropertyChanged(nameof(TopLeftGrowthChance));
            OnPropertyChanged(nameof(TopRightGrowthChance));
            OnPropertyChanged(nameof(BottomRightGrowthChance));
            OnPropertyChanged(nameof(BottomLeftGrowthChance));
        }

        public void IncreaseRegeneration()
        {
            GrowthScorecard.RegenerationChancePercentage +=
                _passiveSkillsData.RegenerationChancePerAttributePoint;

            AvailableMutationPoints--;
        }

        public void IncreaseMycotoxicity()
        {
            GrowthScorecard.MycotoxinFungicideChancePercentage +=
                _passiveSkillsData.MycotoxinFungicideChancePerAttributePoint;

            AvailableMutationPoints--;
        }

        public void IncreaseHydrophilia()
        {
            GrowthScorecard.MoistureGrowthBoost +=
                _passiveSkillsData.MoistureGrowthBoostPerAttributePoint;

            AvailableMutationPoints--;
        }

        public void IncreaseSpores()
        {
            GrowthScorecard.SporesChancePercentage +=
                _passiveSkillsData.SporesChancePerAttributePoint;

            AvailableMutationPoints--;
        }

        public void UseEyeDropper()
        {
            ActionPoints--;
        }

        public void UseDeadCell()
        {
            ActionPoints--;
        }

        public void UseIncreaseLighting()
        {
            ActionPoints--;
        }

        public void UseDecreaseLighting()
        {
            ActionPoints--;
        }

        public bool IsLocalPlayer(List<string> userNames)
        {
            return userNames.Contains(Name);
        }
    }
}