using UnityEngine;
using Unity.Mathematics;

namespace gamer.tetris
{
    [CreateAssetMenu(menuName = "Puzzle")]
    public class Puzzle : ScriptableObject, IPuzzle
    {
        public static readonly float[] rotations = {0f, 90f, 180f, 270f};
        [SerializeField] Tile[] _tiles;
        [SerializeField] int2[] _tilesOffset;
        [SerializeField] float2 _rotationCenter;

        public Tile[] Tiles => _tiles;
        public int TilesCount => _tiles.Length;
        public int RotationCount => 4;
        //todo: tests
        public int2[] GetTileOffset(int rotationIndex)
        {
            if (rotationIndex >= RotationCount || rotationIndex < 0)
                throw new System.ArgumentOutOfRangeException(nameof(rotationIndex), "Invalid rotation.");
            float rotation = rotations[rotationIndex];
            var sin = Mathf.Sin(Mathf.Deg2Rad * rotation);
            var cos = Mathf.Cos(Mathf.Deg2Rad * rotation);

            var offsets = (int2[])_tilesOffset.Clone();
            for (int i = 0; i < _tilesOffset.Length; i++)
            {
                var newX = ((offsets[i].x - _rotationCenter.x) * cos) - ((offsets[i].y - _rotationCenter.y) * sin);
                var newY = ((offsets[i].x - _rotationCenter.x) * sin) + ((offsets[i].y - _rotationCenter.y) * cos);
                offsets[i] = new int2((int)Mathf.Round(newX + _rotationCenter.x), (int)Mathf.Round(newY + _rotationCenter.y));
            }
            return offsets;
        }
    }
}
