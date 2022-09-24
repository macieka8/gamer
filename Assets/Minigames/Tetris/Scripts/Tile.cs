using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    [System.Serializable]
    public class Tile
    {
        public static bool IsNullOrEmpty(Tile tile)
        {
            return tile == null || tile.IsEmpty;
        }

        public static int Distance(int2 a, int2 b)
        {
            var c = a - b;
            return Mathf.Abs(c.x) + Mathf.Abs(c.y);
        }

        [SerializeField] Sprite _sprite;
        [SerializeField] Color _color;

        public Sprite Sprite => _sprite;
        public Color Color => _color;
        public bool IsEmpty => _sprite == null;

        public Tile(Sprite sprite)
        {
            _sprite = sprite;
        }
    }
}
