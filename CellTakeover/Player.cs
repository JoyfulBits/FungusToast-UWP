using Windows.UI;
using Windows.UI.Xaml.Media;

namespace CellTakeover
{
    internal class Player
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int PlayerNumber { get; set; }

        public BioCell MakeCell(int cellIndex)
        {
            return new BioCell(PlayerNumber, cellIndex, Color);
        }
    }
}