using UnityEngine;

namespace gamer.pacman
{
    public class PacmanWorldDrawer : MonoBehaviour
    {
        [SerializeField] GameObject _wallPrefab;
        [SerializeField] GameObject _pointPrefab;

        void Start()
        {
            var layout = PacmanWorld.Instance.Layout;

            for (int y = 0; y < layout.mapDimensions.y; y++)
            {
                for (int x = 0; x < layout.mapDimensions.x; x++)
                {
                    var tile = layout.GetTileAtCoords(x, y);
                    if (tile == PacmanLayout.TileType.Wall)
                    {
                        var wallPosition = layout.GetPositionFromCoords(x, y);
                        var wall = Instantiate(
                            _wallPrefab, transform);
                        wall.transform.localPosition = (Vector2)wallPosition;
                        wall.transform.localScale = (Vector2)layout.tileSize;
                    }
                    else
                    {
                        var pointPosition = layout.GetPositionFromCoords(x, y);
                        var point = Instantiate(
                            _pointPrefab, transform);
                        point.transform.localPosition = (Vector2)pointPosition;
                    }
                }
            }
        }
    }
}
