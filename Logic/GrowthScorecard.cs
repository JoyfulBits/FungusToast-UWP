using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic.Annotations;

namespace Logic
{
    public class GrowthScorecard : INotifyPropertyChanged
    {
        public const double BaseGrowthPercentage = 10;
        public const double BaseCellDeathChanceForSurroundedCells = 5;
        public const double BaseMutationChancePercentage = 10;
        public const double BaseApoptosisChancePercentage = 5;

        public Dictionary<RelativePosition, double> GrowthChanceDictionary = new Dictionary<RelativePosition, double>
        {
            { RelativePosition.TopLeft, 0 },
            { RelativePosition.Top, BaseGrowthPercentage },
            { RelativePosition.TopRight, 0 },
            { RelativePosition.Right, BaseGrowthPercentage },
            { RelativePosition.BottomRight, 0 },
            { RelativePosition.Bottom, BaseGrowthPercentage },
            { RelativePosition.BottomLeft, 0 },
            { RelativePosition.Left, BaseGrowthPercentage }
        };

        private double _starvedCellDeathChancePercentage = BaseCellDeathChanceForSurroundedCells;
        private double _regenerationChancePercentage;
        private double _mutationChancePercentage = BaseMutationChancePercentage;
        private double _apoptosisChancePercentage = BaseApoptosisChancePercentage;
        private double _mycotoxinFungicideChancePercentage;

        /// <summary>
        /// Percentage chance that a surrounded/starved cell will die each generation
        /// </summary>
        public double StarvedCellDeathChancePercentage
        {
            get => _starvedCellDeathChancePercentage;
            set
            {
                if (value == _starvedCellDeathChancePercentage) return;
                _starvedCellDeathChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public double MutationChancePercentage
        {
            get => _mutationChancePercentage;
            set
            {
                if (value == _mutationChancePercentage) return;
                _mutationChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public double ApoptosisChancePercentage
        {
            get => _apoptosisChancePercentage;
            set
            {
                if (Math.Abs(value - _apoptosisChancePercentage) < .0001) return;
                _apoptosisChancePercentage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage chance that an adjacent dead cell will regrow into a live cell
        /// </summary>
        public double RegenerationChancePercentage
        {
            get => _regenerationChancePercentage;
            set
            {
                if (value == _regenerationChancePercentage) return;
                _regenerationChancePercentage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage chance that an adjacent enemy live cell die due to mycotoxins
        /// </summary>
        public double MycotoxinFungicideChancePercentage
        {
            get => _mycotoxinFungicideChancePercentage;
            set
            {
                if (value == _mycotoxinFungicideChancePercentage) return;
                _mycotoxinFungicideChancePercentage = value;
                OnPropertyChanged();
            }
        }


        public double GetGrowthChance(RelativePosition position)
        {
            return GrowthChanceDictionary[position];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
