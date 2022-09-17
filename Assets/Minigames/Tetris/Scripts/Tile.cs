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
        [SerializeField] Sprite _sprite;

        public Sprite Sprite => _sprite;
        public bool IsEmpty => _sprite == null;

        public Tile(Sprite sprite)
        {
            _sprite = sprite;
        }
    }
}
