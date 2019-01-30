using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Logic;
using Logic.Annotations;

namespace FungusToast
{
    public class FungusToastViewModel : INotifyPropertyChanged
    {
        public const int NumberOfTurnsAfterGridIsFullBeforeGameEnds = 5;

        private int _generationNumber;
        private List<IPlayer> _players = new List<IPlayer>();

        private int _gameEndCountDown = NumberOfTurnsAfterGridIsFullBeforeGameEnds;
        private int _totalEmptyCells;
        private int _totalDeadCells;
        private int _totalLiveCells;
        private int _roundNumber;
        private int _totalRegeneratedCells;
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

        public int TotalLiveCells
        {
            get => _totalLiveCells;
            set
            {
                if (value == _totalLiveCells) return;
                _totalLiveCells = value;
                OnPropertyChanged();
            }
        }

        public int TotalDeadCells
        {
            get => _totalDeadCells;
            set
            {
                if (value == _totalDeadCells) return;
                _totalDeadCells = value;
                OnPropertyChanged();
            }
        }

        public int RoundsUntilNextMutation => NumberOfGenerationsBetweenFreeMutations -
                                              GenerationNumber % NumberOfGenerationsBetweenFreeMutations;

        public int TotalEmptyCells  
        {
            get => _totalEmptyCells;
            set
            {
                if (value == _totalEmptyCells) return;
                _totalEmptyCells = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public List<IPlayer> GameOverResult
        {
            get
            {
                return Players.OrderByDescending(x => x.LiveCells).ToList();
            }
        }

        public void TriggerGameOverResultOnPropertyChanged()
        {
            OnPropertyChanged(nameof(GameOverResult));
        }

        public int GameEndCountDown
        {
            get => _gameEndCountDown;
            set
            {
                if (value == 0)
                {
                    OnPropertyChanged(nameof(GameOverResult));
                }
                if (value == _gameEndCountDown) return;
                _gameEndCountDown = value;
                OnPropertyChanged();
            }
        }

        public int RoundNumber
        {
            get => _roundNumber;
            set
            {
                if (value == _roundNumber) return;
                _roundNumber = value;
                OnPropertyChanged();
            }
        }

        public int TotalRegeneratedCells
        {
            get => _totalRegeneratedCells;
            set
            {
                if (value == _totalRegeneratedCells) return;
                _totalRegeneratedCells = value;
                OnPropertyChanged();
            }
        }
    }
}