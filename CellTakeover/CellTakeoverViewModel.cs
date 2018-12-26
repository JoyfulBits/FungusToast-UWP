using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic;
using Logic.Annotations;

namespace CellTakeover
{
    public class CellTakeoverViewModel : INotifyPropertyChanged
    {
        private int _generationNumber;
        private int _totalLiveCells;
        private int _totalDeadCells;
        public Dictionary<int, BioCell> CurrentLiveCells { get; } = new Dictionary<int, BioCell>();
        public Dictionary<int, BioCell> CurrentDeadCells { get; } = new Dictionary<int, BioCell>();

        public List<IPlayer> Players { get; set; } = new List<IPlayer>();

        public int GenerationNumber
        {
            get => _generationNumber;
            set
            {
                if (value == _generationNumber) return;
                _generationNumber = value;
                OnPropertyChanged();
            }
        }

        public int TotalLiveCells => CurrentLiveCells.Count;

        public int TotalDeadCells => CurrentDeadCells.Count;

        /// <summary>
        /// Represents the current pending mutation choices that have to get resolved before the next generation can grow
        /// </summary>
        public Stack<Tuple<IPlayer, MutationChoice>> MutationChoices { get; set; } = new Stack<Tuple<IPlayer, MutationChoice>>();


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddNewLiveCell(int cellIndex, BioCell newCell)
        {
            CurrentLiveCells.Add(cellIndex, newCell);
            OnPropertyChanged(nameof(TotalLiveCells));
        }

        public void AddNewDeadCell(int cellIndex, BioCell newCell)
        {
            CurrentDeadCells.Add(cellIndex, newCell);
            OnPropertyChanged(nameof(TotalDeadCells));
        }

        public void RemoveLiveCell(int cellIndex)
        {
            CurrentLiveCells.Remove(cellIndex);
            OnPropertyChanged(nameof(TotalLiveCells));
        }
    }
}