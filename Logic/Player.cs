using Windows.UI;

namespace Logic
{
    public class Player : IPlayer
    {
        public string Name { get; set; }
        public Color Color { get; set; }
        public int PlayerNumber { get; set; }
        public BioCell MakeCell(int cellIndex)
        {
            return new BioCell(this, cellIndex, Color);
        }
    }
}