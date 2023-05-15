using UnityEngine;
using Unity.Mathematics;

namespace gamer.tetris
{
    public interface IPuzzle
    {
        public Tile[] Tiles { get; }
        public int TilesCount { get; }
        public int RotationCount { get; }
        public int2[] GetTileOffset(int rotation);
    }
}
