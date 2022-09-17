using UnityEngine;
using Unity.Mathematics;

namespace gamer.tetris
{
    [CreateAssetMenu(menuName = "Puzzle")]
    public class Puzzle : ScriptableObject, IPuzzle
    {
        [SerializeField] Tile[] _tiles;
        [SerializeField] int2[] _tilesOffset;

        public Tile[] Tiles => _tiles;
        public int2[] TilesOffset => _tilesOffset;
        public int TilesCount => _tiles.Length;
    }
}
