using Unity.Mathematics;

namespace gamer.pacman
{
    public class Ghost
    {
        PacmanMovement _movement;

        float normalSpeed;
        float fearedSpeed;
        float fearDurationLeft;

        public PacmanMovement Movement => _movement;
        public bool IsFeared { 
            get => fearDurationLeft > 0f;
        }

        public Ghost(float speed, float2 position)
        {
            normalSpeed = speed;
            fearedSpeed = speed / 2f;
            _movement = new PacmanMovement(normalSpeed, position);
        }

        int GetPossibleDirectionsCount(PacmanLayout layout)
        {
            int count = 0;
            for (int i = 0; i < PacmanLayout.DIRECTIONS.Length; i++)
            {
                if (_movement.CanMoveInDirection(PacmanLayout.DIRECTIONS[i], layout))
                {
                    count++;
                }
            }
            return count;
        }

        float2 GetMoveDirectionToPlayer(PacmanMovement player)
        {
            //todo : use pathfinding
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

        public void ChooseNextDirection(PacmanLayout layout, PacmanMovement player)
        {
            float2 newDirection = float2.zero;
            if (IsFeared)
            {
                var directionToPlayer = GetMoveDirectionToPlayer(player);
                if (GetPossibleDirectionsCount(layout) > 2)
                {
                    do
                    {
                        newDirection = PacmanLayout.GetRandomMoveDirection();
                    } while ((newDirection == directionToPlayer).Equals(new bool2(true, true)));
                }
                else
                {
                    do
                    {
                        newDirection = PacmanLayout.GetRandomMoveDirection();
                    } while ((newDirection == -_movement.PreviousDirection).Equals(new bool2(true, true)));
                }
            }
            else
            {
                var isFollowingPlayer = UnityEngine.Random.Range(0, 2) == 0;

                if (isFollowingPlayer && GetPossibleDirectionsCount(layout) > 2)
                {
                    newDirection = GetMoveDirectionToPlayer(player);
                }
                else
                {
                    do
                    {
                        newDirection = PacmanLayout.GetRandomMoveDirection();
                    } while ((newDirection == -_movement.PreviousDirection).Equals(new bool2(true, true)));
                }
            }
            _movement.SetDesiredMoveDirection(newDirection);
        }

        public void Fear(float fearDurationInSeconds)
        {
            fearDurationLeft = fearDurationInSeconds;
            _movement.Speed = fearedSpeed;
        }

        public void ClearFear()
        {
            fearDurationLeft = 0f;
            _movement.Speed = normalSpeed;
        }

        public void UpdateFear(float deltaTime)
        {

            if (IsFeared)
            {
                fearDurationLeft -= deltaTime;
            }
            else
            {
                _movement.Speed = normalSpeed;
            }
        }
    }
}
