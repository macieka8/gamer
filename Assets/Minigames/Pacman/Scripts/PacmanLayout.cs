using Unity.Mathematics;
using System;

namespace gamer.pacman
{
    public class PacmanLayout
    {
        public enum TileType
        {
            Walkable,
            Wall,
            SmallPoint,
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
            int2 coords = new int2(x, y);
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

        public int CountTileTypes(TileType tileType)
        {
            int count = 0;
            for (int i = 0; i < tiles.Length; i++)
            {
                if (tiles[i] == tileType)
                    count++;
            }

            return count;
        }
    }
}
