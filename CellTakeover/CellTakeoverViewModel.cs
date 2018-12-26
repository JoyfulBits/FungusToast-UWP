﻿using System;
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
        public Dictionary<int, BioCell> CurrentLiveCells { get; set; } = new Dictionary<int, BioCell>();
        public Dictionary<int, BioCell> CurrentDeadCells { get; set; } = new Dictionary<int, BioCell>();

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
    }
}