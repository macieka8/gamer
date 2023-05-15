using UnityEngine;
using System;

namespace gamer.pacman
{
    public class PointsCounter : MonoBehaviour
    {
        int _points;

        public static event Action<int> OnPointsCollected;

        public int Points => _points;

        void Start()
        {
            PacmanWorld.Instance.OnPlayerReachedTargetPosition += HandleTargetReached;
            PacmanWorld.Instance.OnPlayerLose += HandlePlayerLose;
            PacmanWorld.Instance.OnGhostEaten += HandleGhostEaten;
        }

        void HandlePlayerLose()
        {
            _points = 0;
        }

        void HandleTargetReached()
        {
            var player = PacmanWorld.Instance.Player;
            var layout = PacmanWorld.Instance.Layout;

            var playerCoords = layout.GetCoordsFromPosition(player.Position);
            var tileType = layout.GetTileAtCoords(playerCoords);

            if (tileType == PacmanLayout.TileType.SmallPoint)
            {
                _points += 10;
                PacmanWorld.Instance.Layout.SetTileAtCoords(playerCoords, PacmanLayout.TileType.Walkable);
                OnPointsCollected?.Invoke(_points);
            }
            else if (tileType == PacmanLayout.TileType.BigPoint)
            {
                _points += 50;
                PacmanWorld.Instance.Layout.SetTileAtCoords(playerCoords, PacmanLayout.TileType.Walkable);
                OnPointsCollected?.Invoke(_points);
            }
        }

        void HandleGhostEaten()
        {
            _points += 200;
        }
    }
}
