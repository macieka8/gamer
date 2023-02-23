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

        [Header("Ghost Options")]
        [SerializeField] int _ghostCount;
        [SerializeField] int2 _ghostSpawnCoords;
        [SerializeField] GhostComponent _ghostPrefab;

        [Header("Player Options")]
        [SerializeField] int2 _playerSpawnCoords;
        [SerializeField] PacmanMovementComponent _playerPrefab;
        [SerializeField] Vector2InputSender _playerMovementInputSender;

        PacmanLayout _layout;
        PacmanMovement _player;
        PacmanMovement[] _ghosts;

        public PacmanLayout Layout => _layout;
        public PacmanMovement Player => _player;
        public PacmanMovement[] Ghosts => _ghosts;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _layout = PacmanLayoutGenerator.GenerateLayout(_layoutDimensions, _tileSize);

                var player = Instantiate(_playerPrefab, transform);
                player.SetInputSender(_playerMovementInputSender);
                _player = player.Movement;

                _ghosts = new PacmanMovement[4];
                for (int i = 0; i < _ghostCount; i++)
                {
                    var ghost = Instantiate(_ghostPrefab, transform);
                    ghost.Movement.Position = _layout.GetPositionFromCoords(_ghostSpawnCoords);
                    Ghosts[i] = ghost.Movement;
                }
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }
    }
}
