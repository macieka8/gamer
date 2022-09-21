using System.Collections.Generic;
using Unity.Mathematics;

namespace gamer.tetris
{
    public class TetrisBoard
    {
        public const int Width = 10;
        public const int Height = 20;
        public const int Size = Width * Height;

        Tile[,] _board = new Tile[Height, Width];

        public bool IsPositionOnBoard(int2 position)
        {
            if (position.x < 0 || position.x >= Width) return false;
            if (position.y < 0 || position.y >= Height) return false;
            return true;
        }

        public Tile GetValue(int x, int y)
        {
            if (x < 0 || x >= Width) throw new System.ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new System.ArgumentOutOfRangeException(nameof(y));
            return _board[y, x];
        }

        public Tile GetValue(int2 position)
        {
            return GetValue(position.x, position.y);
        }

        public void SetValue(int x , int y, Tile value)
        {
            if (x < 0 || x >= Width) throw new System.ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new System.ArgumentOutOfRangeException(nameof(y));
            _board[y, x] = value;
        }

        public void SetValue(int2 position, Tile value)
        {
            SetValue(position.x, position.y, value);
        }

        public void SetValue(int2 position, IPuzzle puzzle, int rotation)
        {
            var offsets = puzzle.GetTileOffset(rotation);
            for (int i = 0; i < puzzle.TilesCount; i++)
            {
                SetValue(position + offsets[i], puzzle.Tiles[i]);
            }
        }

        public bool FitsOnBoard(int2[] tilesOffsets, int2 position)
        {
            for (int i = 0; i < tilesOffsets.Length; i++)
            {
                var testPosition = position + tilesOffsets[i];
                if (!IsPositionOnBoard(testPosition)) return false;
                if (!Tile.IsNullOrEmpty(GetValue(testPosition))) return false;
            }
            return true;
        }
    }
}
