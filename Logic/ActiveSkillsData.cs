namespace Logic
{
    public class ActiveSkillsData
    {
        public ActiveSkillsData(int waterDropletsPerEyeDropperPoint, int numberOfDeadCellsPerDeadCellAction)
        {
            WaterDropletsPerEyeDropperPoint = waterDropletsPerEyeDropperPoint;
            NumberOfDeadCellsPerDeadCellAction = numberOfDeadCellsPerDeadCellAction;
        }

        public int WaterDropletsPerEyeDropperPoint { get; }
        public int NumberOfDeadCellsPerDeadCellAction { get; }


        public string AddWaterDropletMessage =>
            $"Add {WaterDropletsPerEyeDropperPoint} water droplets to the toast, making it moist.";

        public string AddDeadCellMessage =>
            $"Add {NumberOfDeadCellsPerDeadCellAction} dead cell(s) to an empty space on the toast.";
    }
}