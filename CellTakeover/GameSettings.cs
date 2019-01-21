namespace CellTakeover
{
    internal static class GameSettings
    {
        internal static int NumberOfColumnsAndRows = 45;
        internal static int NumberOfCells => NumberOfColumnsAndRows * NumberOfColumnsAndRows;


#if DEV
        public const string BaseURL = "http://localhost:4000";
#else
        public const string BaseURL = "http://some production url";
#endif
    }
}
