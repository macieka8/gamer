using UnityEngine;
using Unity.Mathematics;
using System;

namespace gamer.pacman
{
    public class PacmanMovement
    {
        float2 _position;
        float _speed;
        float2 _desiredMoveDirection;

        float2 _targetPosition;
        public event Action OnTargetPositionReached;

        public float2 Position { get => _position; set => _position = value; }

        public PacmanMovement(float speed, float2 position = default)
        {
            _speed = speed;
            _position = position;
            _desiredMoveDirection = float2.zero;
            _targetPosition = position;
        }

        public void UpdateMove(PacmanLayout layout)
        {
            if (MoveToTarget())
            {
                OnTargetPositionReached?.Invoke();
                var direction = math.normalizesafe(_targetPosition - _position);
                if (CanMoveInDirection(_desiredMoveDirection, layout))
                {
                    var targetCoords = layout.GetCoordsFromPosition(_position) + (int2)_desiredMoveDirection;
                    if (!(_desiredMoveDirection == float2.zero).Equals(new bool2(true, true)))
                        _targetPosition = layout.GetPositionFromCoords(targetCoords);
                }
                else if (!(direction == float2.zero).Equals(new bool2(true, true)) && CanMoveInDirection(direction, layout))
                {
                    var targetCoords = layout.GetCoordsFromPosition(_position) + (int2)_desiredMoveDirection;
                    if (!(_desiredMoveDirection == float2.zero).Equals(new bool2(true, true)))
                        _targetPosition = layout.GetPositionFromCoords(targetCoords);
                }
            }
        }

        bool CanMoveInDirection(float2 direction, PacmanLayout layout)
        {
            var testCoords = layout.GetCoordsFromPosition(_position) + (int2)direction;
            return layout.GetTileAtCoords(testCoords) == PacmanLayout.TileType.Walkable;
        }

        bool MoveToTarget()
        {
            var direction = math.normalizesafe(_targetPosition - _position);
            var nextPosition = _position + Time.deltaTime * _speed * direction;
            _position = nextPosition;

            if ((Vector2)_targetPosition == (Vector2)nextPosition)
            {
                _position = _targetPosition;
                return true;
            }
            
            var directionDot = math.dot(direction, math.normalize(_targetPosition - nextPosition));
            if (directionDot < 0)
            {
                _position = _targetPosition;
                return true;
            }
            return false;
        }

        public void SetDesiredMoveDirection(float2 direction)
        {
            _desiredMoveDirection = direction;
        }
    }
}
