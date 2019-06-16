namespace Logic
{
    public class ActiveSkillsData
    {
        public ActiveSkillsData(int waterDropletsPerEyeDropperPoint)
        {
            WaterDropletsPerEyeDropperPoint = waterDropletsPerEyeDropperPoint;
        }

        public int WaterDropletsPerEyeDropperPoint { get; }


        public string AddWaterDropletMessage =>
            $"Add {WaterDropletsPerEyeDropperPoint} water droplets to the toast, making it moist.";
    }
}