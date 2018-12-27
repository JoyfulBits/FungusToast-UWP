using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Logic.Annotations;

namespace Logic
{
    public class GrowthScorecard : INotifyPropertyChanged
    {
        public const int BaseGrowthPercentage = 10;
        public const int BaseCellDeathChanceForSurroundedCells = 5;
        public const int BaseMutationChancePercentage = 10;
        public const int BaseHealthyCellDeathChancePercentage = 5;

        public Dictionary<RelativePosition, int> GrowthChanceDictionary = new Dictionary<RelativePosition, int>
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

        private int _deathChanceForStarvedCells = BaseCellDeathChanceForSurroundedCells;
        private int _regrowthChancePercentage;
        private int _mutationChancePercentage = BaseMutationChancePercentage;
        private double _healthyCellDeathChancePercentage = BaseHealthyCellDeathChancePercentage;

        /// <summary>
        /// Percentage chance that a surrounded/starved cell will die each generation
        /// </summary>
        public int DeathChanceForStarvedCells
        {
            get => _deathChanceForStarvedCells;
            set
            {
                if (value == _deathChanceForStarvedCells) return;
                _deathChanceForStarvedCells = value;
                OnPropertyChanged();
            }
        }

        public int MutationChancePercentage
        {
            get => _mutationChancePercentage;
            set
            {
                if (value == _mutationChancePercentage) return;
                _mutationChancePercentage = value;
                OnPropertyChanged();
            }
        }

        public double HealthyCellDeathChancePercentage
        {
            get => _healthyCellDeathChancePercentage;
            set
            {
                if (Math.Abs(value - _healthyCellDeathChancePercentage) < .0001) return;
                _healthyCellDeathChancePercentage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Percentage chance that an adjacent dead cell will will regrow into a live cell
        /// </summary>
        public int RegrowthChancePercentage
        {
            get => _regrowthChancePercentage;
            set
            {
                if (value == _regrowthChancePercentage) return;
                _regrowthChancePercentage = value;
                OnPropertyChanged();
            }
        }


        public int GetGrowthChance(RelativePosition position)
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
