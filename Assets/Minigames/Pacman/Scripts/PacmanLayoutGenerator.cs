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
                        tiles[x + y * dimensions.x] = PacmanLayout.TileType.Walkable;
                    }
                }
            }

            return new PacmanLayout(dimensions, tileSize, tiles);
        }
    }
}
