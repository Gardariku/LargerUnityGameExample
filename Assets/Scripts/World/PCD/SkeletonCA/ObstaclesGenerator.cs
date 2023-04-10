namespace World.PCD.SkeletonCA
{
    public class ObstaclesGenerator : CellularAutomata
    {
        public ObstaclesGenerator(byte[,] field, int maxX, int maxY)
            : base(field, maxX, maxY)
        {
        }

        protected override byte[,] TickAlgorithm()
        {
            byte[,] field2 = new byte[_maxX, _maxY];

            // 23/3 - Conway's Game of Life
            // The first number(s) is what is required for a cell to continue.
            // The second number(s) is the requirement for birth.
            for (int y = 0; y < _maxY; y++)
            {
                for (int x = 0; x < _maxX; x++)
                {
                    if (_field[x, y] < 2)
                    {
                        int neighbors = GetNumberOfNeighbors(x, y);
                        if (_field[x, y] == 1 && neighbors < 3)
                        {
                            // unlocks
                            field2[x, y] = 0;
                            continue;
                        }

                        if (_field[x, y] == 0 && neighbors > 4)
                        {
                            // becomes locked
                            field2[x, y] = 1;
                            continue;
                        }

                        // cell dies.
                        field2[x, y] = _field[x, y];
                    }
                }
            }

            return field2;
        }

    }
}
