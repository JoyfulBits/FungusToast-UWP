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

            //newCell.Player.LiveCells++;
            ////--at the moment, the only way there is a previous player is if the cell died. May introduce cell takeovers in the future
            //if (newCell.PreviousPlayer != null)
            //{
            //    newCell.PreviousPlayer.DeadCells--;
            //}
            OnPropertyChanged(nameof(TotalLiveCells));
            OnPropertyChanged(nameof(TotalEmptyCells));
        }

        public void AddNewDeadCell(BioCell newCell)
        {
            CurrentDeadCells.Add(newCell.CellIndex, newCell);
            //newCell.Player.DeadCells++;
            //newCell.Player.LiveCells--;
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
            //regrownCell.Player.RegrownCells++;
            RemoveDeadCell(regrownCell.CellIndex);
            AddNewLiveCell(regrownCell);
            OnPropertyChanged(nameof(TotalLiveCells));
            OnPropertyChanged(nameof(TotalDeadCells));
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
    }
}