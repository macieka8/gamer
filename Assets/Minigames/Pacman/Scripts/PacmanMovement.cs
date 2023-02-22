using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public struct PacmanMovement
    {
        float2 _position;
        float _speed;
        float2 _desiredMoveDirection;

        float2 _targetPosition;
        float2 _currentMoveDirection;

        public float2 Position => _position;

        public PacmanMovement(float speed, float2 position = default)
        {
            _speed = speed;
            _position = position;
            _desiredMoveDirection = float2.zero;
            _currentMoveDirection = float2.zero;
            _targetPosition = position;
        }

        public void UpdateMove(PacmanLayout layout)
        {
            if (MoveToTarget())
            {
                if (CanMoveInDirection(_desiredMoveDirection, layout))
                {
                    _currentMoveDirection = _desiredMoveDirection;
                    var targetCoords = layout.GetCoordsFromPosition(_position) + (int2)_currentMoveDirection;
                    if (!(_currentMoveDirection == float2.zero).Equals(new bool2(true, true)))
                        _targetPosition = layout.GetPositionFromCoords(targetCoords);
                }
                else if (CanMoveInDirection(_currentMoveDirection, layout))
                {
                    var targetCoords = layout.GetCoordsFromPosition(_position) + (int2)_currentMoveDirection;
                    if (!(_currentMoveDirection == float2.zero).Equals(new bool2(true, true)))
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
            var nextPosition = _position + Time.deltaTime * _speed * _currentMoveDirection;
            _position = nextPosition;

            if ((Vector2)_targetPosition == (Vector2)nextPosition)
            {
                _position = _targetPosition;
                return true;
            }
            
            var directionDot = math.dot(_currentMoveDirection, math.normalize(_targetPosition - nextPosition));
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
