using Unity.Mathematics;
using UnityEngine;
using System;

namespace gamer.pacman
{
    public class PacmanWorld : MonoBehaviour
    {
        public const float minSafeDistanceSq = 1f;
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

        GameObject _playerGameObject;
        GameObject[] _ghostGameObjects;

        public Action OnPlayerReachedTargetPosition;

        public PacmanLayout Layout => _layout;
        public PacmanMovement Player => _player;
        public PacmanMovement[] Ghosts => _ghosts;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _layout = PacmanLayoutGenerator.GenerateLayout(_layoutDimensions, _tileSize);
                CreateWorld();
            }
        }

        void Update()
        {
            if (IsPlayerNearGhost())
            {
                HandlePlayerDied();
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }

        void CreateWorld()
        {
            var player = Instantiate(_playerPrefab, transform);
            player.SetInputSender(_playerMovementInputSender);
            _player = player.Movement;
            _player.OnTargetPositionReached += PlayerReachedTargetPosition;
            _playerGameObject = player.gameObject;

            _ghosts = new PacmanMovement[_ghostCount];
            _ghostGameObjects = new GameObject[_ghostCount];
            for (int i = 0; i < _ghostCount; i++)
            {
                var ghost = Instantiate(_ghostPrefab, transform);
                ghost.Movement.Position = _layout.GetPositionFromCoords(_ghostSpawnCoords);
                Ghosts[i] = ghost.Movement;
                _ghostGameObjects[i] = ghost.gameObject;
            }
        }

        void PlayerReachedTargetPosition()
        {
            OnPlayerReachedTargetPosition?.Invoke();
        }

        bool IsPlayerNearGhost()
        {
            for (int i = 0; i < _ghostCount; i++)
            {
                if (math.distancesq(_player.Position, _ghosts[i].Position) < minSafeDistanceSq)
                {
                    return true;
                }
            }
            return false;
        }

        void HandlePlayerDied()
        {
            Destroy(_playerGameObject);
            for (int i = 0; i < _ghostCount; i++)
            {
                Destroy(_ghostGameObjects[i]);
            }

            CreateWorld();
        }
    }
}
