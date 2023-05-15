using Unity.Mathematics;
using System;

namespace gamer.pacman
{
    public class PacmanLayout
    {
        public static readonly float2[] DIRECTIONS = new float2[]
        {
                math.right().xy,
                math.up().xy,
                math.left().xy,
                math.down().xy
        };
        
        public static readonly float2 RIGHT = DIRECTIONS[0];
        public static readonly float2 UP = DIRECTIONS[1];
        public static readonly float2 LEFT = DIRECTIONS[2];
        public static readonly float2 DOWN = DIRECTIONS[3];

        public enum TileType
        {
            Walkable,
            Wall,
            SmallPoint,
            BigPoint
        }

        public float2 tileSize;
        public int2 mapDimensions;
        public TileType[] tiles;

        public event Action<int2> OnTileChanged;
        public event Action OnTilesChanged;

        public PacmanLayout(int2 mapDimensions, float2 tileSize, TileType[] tiles)
        {
            this.mapDimensions = mapDimensions;
            this.tileSize = tileSize;
            this.tiles = tiles;
        }

        public TileType GetTileAtCoords(int x, int y)
        {
            return tiles[y * mapDimensions.x + x];
        }

        public TileType GetTileAtCoords(int2 coords)
        {
            return GetTileAtCoords(coords.x, coords.y);
        }

        public int2 GetCoordsFromPosition(float2 position)
        {
            float2 startPosition = -tileSize * mapDimensions / 2f;
            var positionFromStart = position - startPosition;
            var coords = new int2(math.round(positionFromStart / tileSize));
            return coords;
        }

        public int2 GetCoordsFromPosition(float x, float y)
        {
            return GetCoordsFromPosition(new float2(x, y));
        }

        public float2 GetPositionFromCoords(int x, int y)
        {
            var coords = new int2(x, y);
            float2 startPosition = -tileSize * mapDimensions / 2f;
            return startPosition + (float2)coords * tileSize;
        }

        public float2 GetPositionFromCoords(int2 coords)
        {
            return GetPositionFromCoords(coords.x, coords.y);
        }

        public void SetTileAtCoords(int2 coords, TileType newTileType)
        {
            tiles[coords.x + coords.y * mapDimensions.x] = newTileType;
            OnTileChanged?.Invoke(coords);
        }

        public void OverrideLayoutData(int2 mapDimensions, float2 tileSize, TileType[] tiles)
        {
            this.mapDimensions = mapDimensions;
            this.tileSize = tileSize;
            this.tiles = tiles;
            OnTilesChanged?.Invoke();
        }

        public int CountTileTypes(params TileType[] tileTypes)
        {
            int count = 0;
            for (int i = 0; i < tiles.Length; i++)
            {
                for (int typeIndex = 0; typeIndex < tileTypes.Length; typeIndex++)
                {
                    if (tiles[i] == tileTypes[typeIndex])
                    {
                        count++;
                        break;
                    }
                }
            }

            return count;
        }

        public static float2 GetRandomMoveDirection()
        {
            var rand = UnityEngine.Random.Range(0, 4);
            return DIRECTIONS[rand];
        }
    }
}
