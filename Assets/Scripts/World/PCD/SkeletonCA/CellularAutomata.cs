using System;
using System.Collections.Generic;
using UnityEngine;

namespace World.PCD.SkeletonCA
{
    public abstract class CellularAutomata
    {
        protected int _maxX = 0;
        protected int _maxY = 0;
        protected byte[,] _field;
        protected List<List<Vector2Int>> caverns = new List<List<Vector2Int>>();

        /// <summary>
        /// Instantiates the RuleSet with a copy of the game field, and the maximum X,Y boundaries.
        /// </summary>
        /// <param name="field">int[][] game field</param>
        /// <param name="maxX">Maximum X boundary</param>
        /// <param name="maxY">Maximum Y boundary</param>
        public CellularAutomata(byte[,] field, int maxX, int maxY)
        {
            _field = field;
            _maxX = maxX;
            _maxY = maxY;
        }

        /// <summary>
        /// Returns the number of neighbors for a cell at X,Y.
        /// </summary>
        /// <param name="x">X coordinate of cell to check</param>
        /// <param name="y">Y coordinate of cell to check</param>
        /// <returns>number of neighbors</returns>
        protected int GetNumberOfNeighbors(int x, int y)
        {
            // Returns the number of neighbors for a specific coordinate.
            int neighbors = 0;

            if (x + 1 < _maxX && _field[x + 1, y] == 1)
            {
                neighbors++;
            }

            if (x - 1 >= 0 && _field[x - 1, y] == 1)
            {
                neighbors++;
            }

            if (y + 1 < _maxY && _field[x, y + 1] == 1)
            {
                neighbors++;
            }

            if (y - 1 >= 0 && _field[x, y - 1] == 1)
            {
                neighbors++;
            }

            // diaganols
            if (x + 1 < _maxX && y + 1 < _maxY && _field[x + 1, y + 1] == 1)
            {
                neighbors++;
            }

            if (x + 1 < _maxX && y - 1 >= 0 && _field[x + 1, y - 1] == 1)
            {
                neighbors++;
            }

            if (x - 1 >= 0 && y + 1 < _maxY && _field[x - 1, y + 1] == 1)
            {
                neighbors++;
            }

            if (x - 1 >= 0 && y - 1 >= 0 && _field[x - 1, y - 1] == 1)
            {
                neighbors++;
            }

            return neighbors;
        }

        /// <summary>
        /// Executes one generation of the game field, causing cells to live, die, or give birth.
        /// This is a template method, which calls the concrete method TickAlgorithm() to execute the cell movement details.
        /// </summary>
        public void Tick()
        {
            byte[,] field2 = TickAlgorithm();

            // Copy field = field2.
            Array.Copy(field2, _field, field2.Length);

            //Debug.Log("CA Tick");
        }

        public void Tick(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Tick();
            }
        }

        // TODO: �� ������ ������ ������ ��� ����� ��������� ��������
        public byte[,] GetResult(int num)
        {
            Tick(num);
            int tries = 1;
            while (CountCaverns() > 1)
            {
                tries++;
                Debug.Log("Try �" + tries + ", cause of " + caverns.Count + "caverns");
                caverns = new List<List<Vector2Int>>();
                Tick();

                if (tries > 15)
                {
                    Debug.Log("��� ����� �����-�� ����������");
                    return _field;
                }
            }

            return _field;
        }

        /// <summary>
        /// This is part of the template method Tick() and contains the details for cell activity.
        /// This method examines the current field and returns a new field with the changes for 1 generation.
        /// </summary>
        /// <returns>int[][] new game field</returns>
        protected abstract byte[,] TickAlgorithm();

        public void CheckCaverns()
        {
            IdentifyCaverns();
            if (caverns.Count > 0)
            {
                Debug.Log("Detected " + caverns.Count + " separated caverns");
            }
        }

        int CountCaverns()
        {
            IdentifyCaverns();
            return caverns.Count;
        }

        void FloodFillCavern(int x, int y, byte fill, byte[,] cells)
        {
            if (x < 0 || y < 0 || x >= _maxX || y >= _maxY)
                return;

            if (cells[x, y] == 0)
            {
                caverns[^1].Add(new Vector2Int(x, y));

                cells[x, y] = fill;
                FloodFillCavern(x - 1, y, fill, cells);
                FloodFillCavern(x + 1, y, fill, cells);
                FloodFillCavern(x, y + 1, fill, cells);
                FloodFillCavern(x, y - 1, fill, cells);
            }
        }

        void IdentifyCaverns()
        {
            byte[,] copy = _field.Clone() as byte[,];
            byte fill = byte.MaxValue;

            for (int x = 0; x < _maxX; x++)
            {
                for (int y = 0; y < _maxY; y++)
                {
                    if (copy[x, y] == 0)
                    {
                        caverns.Add(new List<Vector2Int>());
                        FloodFillCavern(x, y, fill, copy);
                        fill--;
                    }
                }
            }
        }
    }
}
