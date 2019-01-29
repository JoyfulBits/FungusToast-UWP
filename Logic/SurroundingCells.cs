using System.Collections.Generic;

namespace Logic
{
    public class SurroundingCells
    {
        public GridCell LeftCell { get; set; }
        public GridCell TopCell { get; set; }
        public GridCell TopLeftCell { get; set; }
        public GridCell TopRightCell { get; set; }
        public GridCell RightCell { get; set; }
        public GridCell BottomRightCell { get; set; }
        public GridCell BottomCell { get; set; }
        public GridCell BottomLeftCell { get; set; }

        public List<GridCell> EmptyCells
        {
            get
            {
                var emptyCells = new List<GridCell>();
                if (TopLeftCell.Empty)
                {
                    emptyCells.Add(TopLeftCell);
                }
                if (TopCell.Empty)
                {
                    emptyCells.Add(TopCell);
                }
                if (TopRightCell.Empty)
                {
                    emptyCells.Add(TopRightCell);
                }
                if (RightCell.Empty)
                {
                    emptyCells.Add(RightCell);
                }
                if (BottomRightCell.Empty)
                {
                    emptyCells.Add(BottomRightCell);
                }
                if (BottomCell.Empty)
                {
                    emptyCells.Add(BottomCell);
                }

                if (BottomLeftCell.Empty)
                {
                    emptyCells.Add(BottomLeftCell);
                }

                if (LeftCell.Empty)
                {
                    emptyCells.Add(LeftCell);
                }

                return emptyCells;
            }
        }

        public bool SurroundedByLiveCells =>
            TopLeftCell.OrganicCell && !TopLeftCell.Dead
                                    && TopCell.OrganicCell && !TopCell.Dead
                                    && TopRightCell.OrganicCell && !TopRightCell.Dead
                                    && RightCell.OrganicCell && !RightCell.Dead
                                    && BottomRightCell.OrganicCell && !BottomRightCell.Dead
                                    && BottomCell.OrganicCell && !BottomCell.Dead
                                    && BottomLeftCell.OrganicCell && !BottomLeftCell.Dead
                                    && LeftCell.OrganicCell && !LeftCell.Dead;

        public Dictionary<string, IPlayer> GetAllSurroundingPlayersWithRegrowth()
        {
            var surroundingPlayers = new Dictionary<string, IPlayer>();

            AddBioCellPlayerWithRegrowth(TopLeftCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(TopCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(TopRightCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(RightCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(BottomRightCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(BottomCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(BottomLeftCell, surroundingPlayers);
            AddBioCellPlayerWithRegrowth(LeftCell, surroundingPlayers);

            return surroundingPlayers;
        }

        private void AddBioCellPlayerWithRegrowth(GridCell cell, Dictionary<string, IPlayer> uniqueSurroundingPlayers)
        {
            if (cell.OrganicCell && !cell.Dead)
            {
                var bioCell = cell as BioCell;

                if (bioCell.Player.GrowthScorecard.RegrowthChancePercentage > 0 && !uniqueSurroundingPlayers.ContainsKey(bioCell.Player.PlayerId))
                {
                    uniqueSurroundingPlayers.Add(bioCell.Player.PlayerId, bioCell.Player);
                }
            }
        }
    }
}