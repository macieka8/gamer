using Unity.Mathematics;

namespace gamer.pacman
{
    public class Ghost
    {
        PacmanMovement _movement;

        public PacmanMovement Movement => _movement;

        public Ghost(float speed, float2 position)
        {
            _movement = new PacmanMovement(speed, position);
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
            var isFollowingPlayer = UnityEngine.Random.Range(0, 2) == 0;
            float2 newDirection;

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

            _movement.SetDesiredMoveDirection(newDirection);
        }

    }
}
