using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic.Annotations;

namespace Logic
{
    public class GrowthScorecard : INotifyPropertyChanged
    {
        public const float BaseGrowthPercentage = 6;
        public const float BaseCellDeathChanceForSurroundedCells = 10;
        public const float BaseMutationChancePercentage = 10;
        public const float BaseApoptosisChancePercentage = 5;

        public Dictionary<RelativePosition, float> GrowthChanceDictionary = new Dictionary<RelativePosition, float>
        {
            { RelativePosition.TopLeft, 0F },
            { RelativePosition.Top, BaseGrowthPercentage },
            { RelativePosition.TopRight, 0F },
            { RelativePosition.Right, BaseGrowthPercentage },
            { RelativePosition.BottomRight, 0F },
            { RelativePosition.Bottom, BaseGrowthPercentage },
            { RelativePosition.BottomLeft, 0F },
            { RelativePosition.Left, BaseGrowthPercentage }
        };

        private float _starvedCellDeathChancePercentage = BaseCellDeathChanceForSurroundedCells;
        private float _regenerationChancePercentage;
        private float _mutationChancePercentage = BaseMutationChancePercentage;
        private float _apoptosisChancePercentage = BaseApoptosisChancePercentage;
        private float _mycotoxinFungicideChancePercentage;
        private float _moistureGrowthBoost;

        /// <summary>
        /// Percentage chance that a surrounded/starved cell will die each generation
        /// </summary>
        public float StarvedCellDeathChancePercentage
        {
            get => _starvedCellDeathChancePercentage;
            set
            {
                if (value.Equals(_starvedCellDeathChancePercentage)) return;
                _starvedCellDeathChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public float MutationChancePercentage
        {
            get => _mutationChancePercentage;
            set
            {
                if (value.Equals(_mutationChancePercentage)) return;
                _mutationChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public float ApoptosisChancePercentage
        {
            get => _apoptosisChancePercentage;
            set
            {
                if (value.Equals(_apoptosisChancePercentage)) return;
                _apoptosisChancePercentage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage chance that an adjacent dead cell will regrow into a live cell
        /// </summary>
        public float RegenerationChancePercentage
        {
            get => _regenerationChancePercentage;
            set
            {
                if (value.Equals(_regenerationChancePercentage)) return;
                _regenerationChancePercentage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage chance that an adjacent enemy live cell die due to mycotoxins
        /// </summary>
        public float MycotoxinFungicideChancePercentage
        {
            get => _mycotoxinFungicideChancePercentage;
            set
            {
                if (value.Equals(_mycotoxinFungicideChancePercentage)) return;
                _mycotoxinFungicideChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public float MoistureGrowthBoost
        {
            get => _moistureGrowthBoost;
            set
            {
                if (value.Equals(_moistureGrowthBoost)) return;
                _moistureGrowthBoost = value;
                OnPropertyChanged();
            }
        }


        public float GetGrowthChance(RelativePosition position)
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
