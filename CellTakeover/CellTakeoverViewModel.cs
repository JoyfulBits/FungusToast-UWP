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
        public List<Player> Players { get; set; } = new List<Player>();

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

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}