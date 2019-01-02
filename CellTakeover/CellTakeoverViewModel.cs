﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Logic;
using Logic.Annotations;

namespace CellTakeover
{
    public class CellTakeoverViewModel : INotifyPropertyChanged
    {
        private int _generationNumber;
        private List<IPlayer> _players = new List<IPlayer>();
        public Dictionary<int, BioCell> CurrentLiveCells { get; } = new Dictionary<int, BioCell>();
        public Dictionary<int, BioCell> CurrentDeadCells { get; } = new Dictionary<int, BioCell>();

        public List<IPlayer> Players
        {
            get => _players;
            set
            {
                if (Equals(value, _players)) return;
                _players = value;
                OnPropertyChanged();

            }
        }

        public int NumberOfGenerationsBetweenFreeMutations { get; } = 5;

        public int GenerationNumber
        {
            get => _generationNumber;
            set
            {
                if (value == _generationNumber) return;
                _generationNumber = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RoundsUntilNextMutation));
            }
        }

        public int TotalLiveCells => CurrentLiveCells.Count;

        public int TotalDeadCells => CurrentDeadCells.Count;

        public int RoundsUntilNextMutation => NumberOfGenerationsBetweenFreeMutations -
                                              GenerationNumber % NumberOfGenerationsBetweenFreeMutations;

        public int TotalEmptyCells =>
            GameSettings.NumberOfCells - CurrentLiveCells.Count - CurrentDeadCells.Count;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AddNewLiveCell(BioCell newCell)
        {
            CurrentLiveCells.Add(newCell.CellIndex, newCell);
            OnPropertyChanged(nameof(TotalLiveCells));
            OnPropertyChanged(nameof(TotalEmptyCells));
        }

        public void AddNewDeadCell(BioCell newCell)
        {
            CurrentDeadCells.Add(newCell.CellIndex, newCell);
            OnPropertyChanged(nameof(TotalDeadCells));
            OnPropertyChanged(nameof(TotalEmptyCells));
        }

        public void RemoveLiveCell(int cellIndex)
        {
            CurrentLiveCells.Remove(cellIndex);
            OnPropertyChanged(nameof(TotalLiveCells));
            OnPropertyChanged(nameof(TotalEmptyCells));
        }

        public void RemoveDeadCell(int cellIndex)
        {
            CurrentDeadCells.Remove(cellIndex);
            OnPropertyChanged(nameof(TotalDeadCells));
            OnPropertyChanged(nameof(TotalEmptyCells));
        }

        public void RegrowCell(BioCell regrownCell)
        {
            RemoveDeadCell(regrownCell.CellIndex);
            AddNewLiveCell(regrownCell);
            OnPropertyChanged(nameof(TotalLiveCells));
            OnPropertyChanged(nameof(TotalDeadCells));
        }
    }
}