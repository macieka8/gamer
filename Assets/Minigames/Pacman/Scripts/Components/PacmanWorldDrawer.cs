using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public class PacmanWorldDrawer : MonoBehaviour
    {
        [SerializeField] GameObject _wallPrefab;
        [SerializeField] GameObject _pointPrefab;

        GameObject[] _tiles;

        void Start()
        {
            var layout = PacmanWorld.Instance.Layout;
            _tiles = new GameObject[layout.mapDimensions.x * layout.mapDimensions.y];

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
                        _tiles[x + y * layout.mapDimensions.x] = wall;
                    }
                    else if (tile == PacmanLayout.TileType.SmallPoint)
                    {
                        var pointPosition = layout.GetPositionFromCoords(x, y);
                        var point = Instantiate(
                            _pointPrefab, transform);
                        point.transform.localPosition = (Vector2)pointPosition;
                        _tiles[x + y * layout.mapDimensions.x] = point;
                    }
                }
            }

            PacmanWorld.Instance.Layout.OnTileChanged += HandleLayoutChanged;
        }

        void HandleLayoutChanged(int2 coords)
        {
            var tileType = PacmanWorld.Instance.Layout.GetTileAtCoords(coords);
            var objectToChange = _tiles[coords.x + coords.y * PacmanWorld.Instance.Layout.mapDimensions.x];
            if (tileType == PacmanLayout.TileType.Walkable)
            {
                if (objectToChange != null)
                    Destroy(objectToChange);
            }
        }
    }
}
