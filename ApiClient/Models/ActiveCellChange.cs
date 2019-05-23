namespace ApiClient.Models
{
    public class ActiveCellChange
    {
        public int GridCellIndex { get; }
        public ActiveCellChangeType MoistureDroplet { get; }
        public string ActivePlayerId { get; }

        public ActiveCellChange(string activePlayerId, int gridCellIndex, ActiveCellChangeType moistureDroplet)
        {
            ActivePlayerId = activePlayerId;
            GridCellIndex = gridCellIndex;
            MoistureDroplet = moistureDroplet;
        }
    }
}
