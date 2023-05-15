using System.Collections.Generic;
using Unity.Mathematics;

namespace gamer.pacman
{
    public static class PacmanLayoutGenerator
    {
        public static PacmanLayout GenerateLayout(float2 tileSize)
        {
            var tetrisMap = GetTetrisPiecesMap5x9();
            return GetLayoutFromTetrisMap(tetrisMap, tileSize);
        }

        public static int[] GetTetrisPiecesMap5x9()
        {
            #region Example Map
            // Example map
            //int[] map =
            //{
            //     1,  1,  7, 7,  11,
            //     1,  2,  7, 7,  11,
            //     2,  2,  8, 8,  8,
            //     3,  2,  8, 9,  12,
            //     3,  3,  3, 9,  9,
            //     4,  6,  6, 9,  13,
            //     4,  4,  6, 10, 13,
            //     4,  5,  6, 10, 10,
            //     5,  5,  5, 10, 14,
            //};
            #endregion

            const int maxPieceSize = 5;
            int[] map = new int[5 * 9];
            int currentlyFilledCells = 0;
            int uniqueNumber = 0; // Used to identify a piece
            while (currentlyFilledCells != map.Length)
            {
                uniqueNumber++;
                var cellsLeftToFill = UnityEngine.Random.Range(
                    math.min(map.Length - currentlyFilledCells, 3),
                    math.min(map.Length - currentlyFilledCells, maxPieceSize));
                var emptyNeighbors = new List<int>();

                int emptyCellIndex = 0;
                while (map[emptyCellIndex] != 0)
                    emptyCellIndex++;

                emptyNeighbors.Add(emptyCellIndex);
                while (emptyNeighbors.Count > 0)
                {
                    // Get Random empty neighbor
                    var randomEmptyNeighborIndex = UnityEngine.Random.Range(0, emptyNeighbors.Count);
                    var randomNeighborCellIndex = emptyNeighbors[randomEmptyNeighborIndex];
                    // Fill empty cell
                    emptyNeighbors.RemoveAt(randomEmptyNeighborIndex);
                    map[randomNeighborCellIndex] = uniqueNumber;
                    cellsLeftToFill--;
                    currentlyFilledCells++;

                    if (cellsLeftToFill == 0)
                        break;

                    // Get empty neighboring cells
                    var neighbors = GetEmptyNeighbors(randomNeighborCellIndex);
                    for (int i = 0; i < neighbors.Count; i++)
                    {
                        if (!emptyNeighbors.Contains(neighbors[i]))
                            emptyNeighbors.Add(neighbors[i]);
                    }
                }
            }

            return map;
            #region Local Functions
            List<int> GetEmptyNeighbors(int cellIndex)
            {
                var neighbors = new List<int>();
                var coords = GetCoords(cellIndex);

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (math.abs(x) + math.abs(y) == 1)
                        {
                            var neighborCoords = new int2(coords.x + x, coords.y + y);
                            if (IsLegalCoords(neighborCoords) && GetMapValueFromCoords(neighborCoords) == 0)
                                neighbors.Add(GetIndexFromCoords(neighborCoords));
                        }
                    }
                }
                return neighbors;
            }

            int2 GetCoords(int cellIndex)
            {
                return new int2(cellIndex % 5, cellIndex / 5);
            }

            int GetIndexFromCoords(int2 coords)
            {
                return coords.x + coords.y * 5;
            }

            int GetMapValueFromCoords(int2 coords)
            {
                return map[GetIndexFromCoords(coords)];
            }

            bool IsLegalCoords(int2 coords)
            {
                return coords.x >= 0 && coords.x < 5
                 && coords.y >= 0 && coords.y < 9;
            }
            #endregion
        }

        public static PacmanLayout GetLayoutFromTetrisMap(int[] tetrisMap, float2 tileSize)
        {
            // (5 {tetrisMap} * 3 {up-scaling} + 1 {for border}) * 2 {for mirroring}
            // 9 {tetrisMap} * 3 {up-scaling} + 3 {+2 for border & +1 for upper walkable line}
            var dimensions = new int2((5 * 3 + 1) * 2, 9 * 3 + 3);

            PacmanLayout.TileType[] tiles = new PacmanLayout.TileType[dimensions.x * dimensions.y];
            
            // Generate Left side of layout
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x / 2; x++)
                {
                    var tetrisMapCoords = new int2((x - 1) / 3, (y - 1) / 3);

                    int currentNumber;
                    if (tetrisMapCoords.x + tetrisMapCoords.y * 5 >= tetrisMap.Length)
                        currentNumber = 0;
                    else
                        currentNumber = tetrisMap[tetrisMapCoords.x + tetrisMapCoords.y * 5];

                    int bottomNumber = 0, leftNumber = 0, bottomLeftNumber = 0;

                    // Get Bottom & Left & Bottom-Left Tetris Numbers
                    if (tetrisMapCoords.x != 0 && tetrisMapCoords.y < 9)
                        leftNumber = tetrisMap[tetrisMapCoords.x - 1 + tetrisMapCoords.y * 5];

                    if (tetrisMapCoords.y != 0)
                        bottomNumber = tetrisMap[tetrisMapCoords.x + (tetrisMapCoords.y - 1) * 5];

                    if (tetrisMapCoords.x != 0 && tetrisMapCoords.y != 0)
                        bottomLeftNumber = tetrisMap[tetrisMapCoords.x - 1 + (tetrisMapCoords.y - 1) * 5];

                    var tileType = PacmanLayout.TileType.Wall;

                    // Set TileType
                    // border
                    if (y == dimensions.y - 1 || x == 0 || y == 0)
                    {
                        tileType = PacmanLayout.TileType.Wall;
                    }
                    // Big points
                    else if (x == 1 && (y == 1 || y == dimensions.y - 2))
                    {
                        tileType = PacmanLayout.TileType.BigPoint;
                    }
                    // ghost house
                    // todo: check if ghost house is not surrounded by walls
                    else if (x >= dimensions.x / 2 - 2 && y < dimensions.y / 2 + 2 && y > dimensions.y / 2 - 2)
                    {
                        tileType = PacmanLayout.TileType.Walkable;
                    }
                    // crossroads
                    else if ((x - 1) % 3 == 0 && (y - 1) % 3 == 0)
                    {
                        if (currentNumber != bottomNumber || currentNumber != leftNumber || currentNumber != bottomLeftNumber)
                        {
                            tileType = PacmanLayout.TileType.SmallPoint;
                        }
                    }
                    // bottom line
                    else if ((x - 1) % 3 == 0)
                    {
                        if (currentNumber != leftNumber)
                            tileType = PacmanLayout.TileType.SmallPoint;
                    }
                    // left line
                    else if ((y - 1) % 3 == 0)
                    {
                        if (currentNumber != bottomNumber)
                            tileType = PacmanLayout.TileType.SmallPoint;
                    }
                    // middle
                    else
                    {
                        tileType = PacmanLayout.TileType.Wall;
                    }

                    tiles[x + y * dimensions.x] = tileType;
                }
            }

            // Mirror left side
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x / 2; x++)
                {
                    tiles[y * dimensions.x + dimensions.x - 1 - x] = tiles[y * dimensions.x + x];
                }
            }
            return new PacmanLayout(dimensions, tileSize, tiles);
        }
    }
}
