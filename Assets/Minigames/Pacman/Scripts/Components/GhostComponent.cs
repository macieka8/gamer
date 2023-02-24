using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public class GhostComponent : MonoBehaviour
    {
        [SerializeField] float _speed;

        PacmanMovement _movement;

        public PacmanMovement Movement => _movement;

        void Awake()
        {
            _movement = new PacmanMovement(_speed, (Vector2)transform.localPosition);
            _movement.SetDesiredMoveDirection(GetRandomMoveDirection());
        }

        void OnEnable()
        {
            _movement.OnTargetPositionReached += HandleTargetReached;
        }

        void OnDisable()
        {
            _movement.OnTargetPositionReached -= HandleTargetReached;
        }

        void Update()
        {
            _movement.UpdateMove(PacmanWorld.Instance.Layout);
            transform.localPosition = (Vector2)_movement.Position;
        }

        void HandleTargetReached()
        {
            var isFollowingPlayer = UnityEngine.Random.Range(0, 2) == 0;
            if (isFollowingPlayer)
                _movement.SetDesiredMoveDirection(GetMoveDirectionToPlayer());
            else
                _movement.SetDesiredMoveDirection(GetRandomMoveDirection());
        }

        float2 GetRandomMoveDirection()
        {
            var rand = UnityEngine.Random.Range(0, 4);
            var randDirection =
                  rand == 0 ? math.right().xy
                : rand == 1 ? math.up().xy
                : rand == 2 ? math.left().xy
                : math.down().xy;

            return randDirection;
        }

        float2 GetMoveDirectionToPlayer()
        {
            //todo : use pathfinding
            var player = PacmanWorld.Instance.Player;

            var directionToPlayer = math.normalize(player.Position - _movement.Position);

            var upFactor = math.dot(math.up().xy, directionToPlayer);
            var leftFactor = math.dot(math.left().xy, directionToPlayer);
            var rightFactor = math.dot(math.right().xy, directionToPlayer);
            var downFactor = math.dot(math.down().xy, directionToPlayer);

            var maxFactor = upFactor;
            var maxDirection = math.up().xy;
            if (leftFactor > maxFactor)
            {
                maxFactor = leftFactor;
                maxDirection = math.left().xy;
            }
            if (rightFactor > maxFactor)
            {
                maxFactor = rightFactor;
                maxDirection = math.right().xy;
            }
            if (downFactor > maxFactor)
            {
                maxDirection = math.down().xy;
            }

            return maxDirection;
        }
    }
}
