using Unity.Mathematics;

namespace gamer.pacman
{
    public static class PacmanLayoutGenerator
    {
        public static PacmanLayout GenerateLayout(int2 dimensions, float2 tileSize)
        {
            PacmanLayout.TileType[] tiles = new PacmanLayout.TileType[dimensions.x * dimensions.y];
            for (int y = 0; y < dimensions.y; y++)
            {
                for (int x = 0; x < dimensions.x; x++)
                {
                    // Generate borders
                    if (x == 0 || y == 0 || x == dimensions.x - 1 || y == dimensions.y - 1 || 
                        (x == 5 && y > 6 && y < 20))
                    {
                        tiles[x + y * dimensions.x] = PacmanLayout.TileType.Wall;
                    }
                    else
                    {
                        tiles[x + y * dimensions.x] = PacmanLayout.TileType.SmallPoint;
                    }
                }
            }

            return new PacmanLayout(dimensions, tileSize, tiles);
        }

        public static int[] GetTetrisPiecesMap5x9()
        {
            int[] map =
            {
                 1,  1,  7, 7,  11,
                 1,  2,  7, 7,  11,
                 2,  2,  8, 8,  8,
                 3,  2,  8, 9,  12,
                 3,  3,  3, 9,  9,
                 4,  6,  6, 9,  13,
                 4,  4,  6, 10, 13,
                 4,  5,  6, 10, 10,
                 5,  5,  5, 10, 14,
            };

            return map;
        }

        public static PacmanLayout GetLayoutFromTetrisMap(int[] tetrisMap)
        {
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

                    //border
                    if (y == dimensions.y - 1 || x == 0 || y == 0)
                    {
                        tileType = PacmanLayout.TileType.Wall;
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
            return new PacmanLayout(dimensions, new float2(0.4f, 0.4f), tiles);
        }
    }
}
