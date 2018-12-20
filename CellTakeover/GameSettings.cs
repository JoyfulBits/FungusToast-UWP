using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellTakeover
{
    public static class GameSettings
    {
        public static int NumberOfColumnsAndRows = 50;
        public static int NumberOfCells => NumberOfColumnsAndRows * NumberOfColumnsAndRows;
    }
}
