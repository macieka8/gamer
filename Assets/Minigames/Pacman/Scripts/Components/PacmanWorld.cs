using Unity.Mathematics;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace gamer.pacman
{
    public class PacmanWorld : MonoBehaviour
    {
        public const float minSafeDistanceSq = 0.25f;
        static PacmanWorld _instance;
        public static PacmanWorld Instance => _instance;

        [Header("Layout Options")]
        [SerializeField] float2 _tileSize;

        [Header("Ghost Options")]
        [SerializeField] int _ghostCount;
        [SerializeField] float _ghostFearDurationInSeconds;
        [SerializeField] GhostComponent _ghostPrefab;

        [Header("Player Options")]
        [SerializeField] int _startingLives;
        [SerializeField] PacmanMovementComponent _playerPrefab;
        [SerializeField] Vector2InputSender _playerMovementInputSender;

        [SerializeField] UnityEvent _onPlayerLose;

        int _lives;
        float2 _ghostSpawnPosition;

        PacmanLayout _layout;
        PacmanMovement _player;
        Ghost[] _ghosts;

        GameObject _playerGameObject;
        GameObject[] _ghostGameObjects;

        public Action OnPlayerReachedTargetPosition;
        public Action<int> OnPlayerLiveChanged;
        public Action OnPlayerLose;
        public Action OnGhostEaten;

        public PacmanLayout Layout => _layout;
        public PacmanMovement Player => _player;
        public Ghost[] Ghosts => _ghosts;

        public int StartingLives => _startingLives;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                
                _lives = _startingLives;
                OnPlayerLiveChanged?.Invoke(_lives);

                _layout = PacmanLayoutGenerator.GenerateLayout(_tileSize);
                _ghostSpawnPosition = _layout.GetPositionFromCoords(_layout.mapDimensions / 2);
                CreateWorldEntities();
            }
        }

        void Update()
        {
            if (IsPlayerNearGhost(out var ghost))
            {
                if (ghost.IsFeared)
                    HandleGhostEaten(ghost);
                else
                    HandlePlayerDied();
            }
        }

        void OnDestroy()
        {
            _instance = null;
        }

        void CreateWorldEntities()
        {
            var player = Instantiate(_playerPrefab, transform);
            player.SetInputSender(_playerMovementInputSender);
            _player = player.Movement;
            _player.Position = _layout.GetPositionFromCoords(new int2 (_layout.mapDimensions.x / 2, 1));
            _player.OnTargetPositionReached += PlayerReachedTargetPosition;
            _playerGameObject = player.gameObject;

            _ghosts = new Ghost[_ghostCount];
            _ghostGameObjects = new GameObject[_ghostCount];
            for (int i = 0; i < _ghostCount; i++)
            {
                var ghost = Instantiate(_ghostPrefab, transform);
                _ghosts[i] = ghost.Ghost;
                _ghosts[i].Movement.Position = _ghostSpawnPosition;
                _ghostGameObjects[i] = ghost.gameObject;
            }
        }

        void CreateNewWorld()
        {
            var newLayout = PacmanLayoutGenerator.GenerateLayout(_tileSize);
            _layout.OverrideLayoutData(newLayout.mapDimensions, newLayout.tileSize, newLayout.tiles);
        }

        void PlayerReachedTargetPosition()
        {
            if (_layout.GetTileAtCoords(_layout.GetCoordsFromPosition(_player.Position)) == PacmanLayout.TileType.BigPoint)
            {
                // Fear all ghosts
                for (int i = 0; i < _ghosts.Length; i++)
                {
                    _ghosts[i].Fear(_ghostFearDurationInSeconds);
                }
            }
            OnPlayerReachedTargetPosition?.Invoke();
            if (_layout.CountTileTypes(PacmanLayout.TileType.SmallPoint, PacmanLayout.TileType.BigPoint) == 0)
            {
                DestroyAllWorldEntiies();
                CreateWorldEntities();
                CreateNewWorld();
            }
        }

        bool IsPlayerNearGhost(out Ghost ghost)
        {
            for (int i = 0; i < _ghostCount; i++)
            {
                if (math.distancesq(_player.Position, _ghosts[i].Movement.Position) < minSafeDistanceSq)
                {
                    ghost = _ghosts[i];
                    return true;
                }
            }
            ghost = null;
            return false;
        }

        void DestroyAllWorldEntiies()
        {
            Destroy(_playerGameObject);
            for (int i = 0; i < _ghostCount; i++)
            {
                Destroy(_ghostGameObjects[i]);
            }
        }

        void HandlePlayerDied()
        {
            DestroyAllWorldEntiies();
            CreateWorldEntities();
            
            _lives--;
            if (_lives == 0)
                HandleZeroLives();

            OnPlayerLiveChanged?.Invoke(_lives);
        }

        void HandleGhostEaten(Ghost ghost)
        {
            ghost.Movement.Position = _ghostSpawnPosition;
            ghost.ClearFear();
            OnGhostEaten?.Invoke();
        }

        void HandleZeroLives()
        {
            _lives = _startingLives;
            
            OnPlayerLiveChanged?.Invoke(_lives);
            OnPlayerLose?.Invoke();
            _onPlayerLose.Invoke();

            CreateNewWorld();
        }
    }
}
