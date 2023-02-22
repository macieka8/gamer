using Unity.Mathematics;
using UnityEngine;

namespace gamer.pacman
{
    public class PacmanWorld : MonoBehaviour
    {
        static PacmanWorld _instance;
        public static PacmanWorld Instance => _instance;

        [Header("Layout Options")]
        [SerializeField] float2 _tileSize;
        [SerializeField] int2 _layoutDimensions;

        PacmanLayout _layout;

        public PacmanLayout Layout => _layout;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _layout = PacmanLayoutGenerator.GenerateLayout(_layoutDimensions, _tileSize);
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }
    }
}
