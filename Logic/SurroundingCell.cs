namespace Logic
{
    public class SurroundingCell
    {
        public GridCell Cell { get; }
        public RelativePosition RelativePosition { get; }

        public SurroundingCell(GridCell cell, RelativePosition relativePosition)
        {
            Cell = cell;
            RelativePosition = relativePosition;
        }
    }
}