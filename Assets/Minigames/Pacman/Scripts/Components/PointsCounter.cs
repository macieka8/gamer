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
            _points = 0;
            PacmanWorld.Instance.OnPlayerReachedTargetPosition += HandleTargetReached;
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
        }
    }
}
