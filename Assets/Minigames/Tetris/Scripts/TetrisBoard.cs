using System.Collections.Generic;
using Unity.Mathematics;

namespace gamer.tetris
{
    public class TetrisBoard : IReadonlyTetrisBoard
    {
        public const int Width = 10;
        public const int Height = 20;
        public const int Size = Width * Height;

        Tile[,] _board = new Tile[Height, Width];

        public event System.Action OnTileOverridden;

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

        public void SetValue(int x , int y, Tile value, bool checkIfOverridden = false)
        {
            if (x < 0 || x >= Width) throw new System.ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= Height) throw new System.ArgumentOutOfRangeException(nameof(y));
            if (checkIfOverridden && !Tile.IsNullOrEmpty(_board[y,x]) && !Tile.IsNullOrEmpty(value))
                OnTileOverridden?.Invoke();
            _board[y, x] = value;
        }

        public void SetValue(int2 position, Tile value,bool checkIfOverridden = false)
        {
            SetValue(position.x, position.y, value, checkIfOverridden);
        }

        public void SetValue(IPuzzle puzzle, int2 position, int rotation, bool checkIfOverridden = false)
        {
            var offsets = puzzle.GetTileOffset(rotation);
            for (int i = 0; i < puzzle.TilesCount; i++)
            {
                SetValue(position + offsets[i], puzzle.Tiles[i], checkIfOverridden);
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

        public int[] GetFilledRows()
        {
            var filledRows = new List<int>();
            for (int rowId = 0; rowId < Height; rowId++)
            {
                bool isFilled = true;
                for (int x = 0; x < Width; x++)
                {
                    if (Tile.IsNullOrEmpty(GetValue(x, rowId)))
                    {
                        isFilled = false;
                        break;
                    }
                }
                if (isFilled)
                {
                    filledRows.Add(rowId);
                }
            }

            return filledRows.ToArray();
        }

        public void DestroyRow(int rowId)
        {
            for (int i = rowId; i > 0; i--)
            {
                for (int x = 0; x < Width; x++)
                {
                    SetValue(x, i, GetValue(x, i - 1));
                }
            }
        }

        public void Clear()
        {
            _board = new Tile[Height, Width];
        }
    }
}
