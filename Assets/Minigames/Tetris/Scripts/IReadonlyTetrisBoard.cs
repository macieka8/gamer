using System.Collections.Generic;
using Unity.Mathematics;

namespace gamer.tetris
{
    public interface IReadonlyTetrisBoard
    {
        public bool IsPositionOnBoard(int2 position);
        public Tile GetValue(int x, int y);
        public Tile GetValue(int2 position);
        public bool FitsOnBoard(int2[] tilesOffsets, int2 position);
        public int[] GetFilledRows();
        public event System.Action OnTileOverridden;
    }
}
