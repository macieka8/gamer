using Unity.Mathematics;

namespace gamer.tetris
{
    public interface IPuzzle
    {
        public Tile[] Tiles { get; }
        public int2[] TilesOffset { get; }
        public int TilesCount { get; }
    }
}
