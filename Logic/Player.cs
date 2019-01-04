using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI;
using Logic.Annotations;

namespace Logic
{
    public class Player : IPlayer
    {
        private readonly Random _random = new Random();
        private readonly ICellGrowthCalculator _cellGrowthCalculator;
        private readonly ISurroundingCellCalculator _surroundingCellCalculator;
        private Color _color;
        private int _playerNumber;
        private GrowthScorecard _growthScorecard;
        private int _liveCells;
        private int _deadCells;

        private string _name;
        private int _regrownCells;
        private int _availableMutationPoints;

        public Player(string name, Color playerCellColor, int playerNumber, 
            ICellGrowthCalculator cellGrowthCalculator, 
            ISurroundingCellCalculator surroundingCellCalculator)
        {
            Name = name;
            Color = playerCellColor;
            PlayerNumber = playerNumber;
            _cellGrowthCalculator = cellGrowthCalculator;
            _surroundingCellCalculator = surroundingCellCalculator;
            _growthScorecard = new GrowthScorecard();
        }

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

        public int PlayerNumber
        {
            get => _playerNumber;
            set
            {
                if (value == _playerNumber) return;
                _playerNumber = value;
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
            }
        }

        public int TopLeftGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft];
        public int TopGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Top];
        public int TopRightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopRight];
        public int RightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Right];
        public int BottomRightGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomRight];
        public int BottomGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Bottom];
        public int BottomLeftGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomLeft];
        public int LeftGrowthChance => GrowthScorecard.GrowthChanceDictionary[RelativePosition.Left];

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

        public int RegrownCells
        {
            get => _regrownCells;
            set
            {
                if (value == _regrownCells) return;
                _regrownCells = value;
                OnPropertyChanged();
            }
        }


        public BioCell MakeCell(int cellIndex)
        {
            return new BioCell(this, cellIndex, Color, _surroundingCellCalculator);
        }

        public BioCell RegrowCell(BioCell deadCell)
        {
            return MakeCell(deadCell.CellIndex);
        }

        public CellGrowthResult CalculateCellGrowth(BioCell cell, SurroundingCells surroundingCells)
        {
            return _cellGrowthCalculator.CalculateCellGrowth(cell, this, surroundingCells);
        }

        public bool GetsFreeMutation()
        {
            return _random.Next(0, 99) < GrowthScorecard.MutationChancePercentage;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string AddMutationChanceMessage => MutationOptionGenerator.IncreaseMutationChanceMessage;
        public string AddCornerGrowthChanceMessage => MutationOptionGenerator.IncreaseCornerGrowthChanceMessage;
        public string ReduceCellDeathChanceMessage => MutationOptionGenerator.DecreaseCellDeathChanceMessage;

        public string AddRegrowthChanceMessage => MutationOptionGenerator.IncreaseRegrowthChanceMessage;

        public void IncreaseMutationChance()
        {
            GrowthScorecard.MutationChancePercentage += MutationOptionGenerator.AdditionalMutationPercentageChancePerAttributePoint;
            AvailableMutationPoints--;
        }

        public void DecreaseHealthyCellDeathChance()
        {
            GrowthScorecard.HealthyCellDeathChancePercentage -= MutationOptionGenerator.ReducedCellDeathPercentagePerAttributePoint;
            AvailableMutationPoints--;
        }

        public void IncreaseCornerGrowth()
        {
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopLeft] +=
                MutationOptionGenerator.AdditionalCornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.TopRight] +=
                MutationOptionGenerator.AdditionalCornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomRight] +=
                MutationOptionGenerator.AdditionalCornerGrowthChancePerAttributePoint;
            GrowthScorecard.GrowthChanceDictionary[RelativePosition.BottomLeft] +=
                MutationOptionGenerator.AdditionalCornerGrowthChancePerAttributePoint;

            AvailableMutationPoints--;

            OnPropertyChanged(nameof(TopLeftGrowthChance));
            OnPropertyChanged(nameof(TopRightGrowthChance));
            OnPropertyChanged(nameof(BottomRightGrowthChance));
            OnPropertyChanged(nameof(BottomLeftGrowthChance));
        }

        public void IncreaseRegrowthChance()
        {
            GrowthScorecard.RegrowthChancePercentage +=
                MutationOptionGenerator.AdditionalRegrowthChancePerAttributePoint;

            AvailableMutationPoints--;
        }
    }
}