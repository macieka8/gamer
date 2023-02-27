using UnityEngine;
using Unity.Mathematics;

namespace gamer.pacman
{
    public class PacmanWorldDrawer : MonoBehaviour
    {
        [SerializeField] GameObject _wallPrefab;
        [SerializeField] GameObject _pointPrefab;
        [SerializeField] GameObject _bigPointPrefab;

        GameObject[] _tiles;

        void Start()
        {
            BuildLayout();

            PacmanWorld.Instance.Layout.OnTileChanged += HandleTileChanged;
            PacmanWorld.Instance.Layout.OnTilesChanged += HandleLayoutChanged;
        }

        void DestroyTiles()
        {
            if (_tiles != null)
            {
                for (int i = 0; i < _tiles.Length; i++)
                {
                    if (_tiles[i] != null)
                        Destroy(_tiles[i]);
                }
            }
        }

        void BuildLayout()
        {
            DestroyTiles();
            var layout = PacmanWorld.Instance.Layout;
            _tiles = new GameObject[layout.mapDimensions.x * layout.mapDimensions.y];

            for (int y = 0; y < layout.mapDimensions.y; y++)
            {
                for (int x = 0; x < layout.mapDimensions.x; x++)
                {
                    var tile = layout.GetTileAtCoords(x, y);
                    if (tile == PacmanLayout.TileType.Wall)
                    {
                        CreateWall(new int2(x, y));
                    }
                    else if (tile == PacmanLayout.TileType.SmallPoint)
                    {
                        CreateSmallPoint(new int2(x, y));
                    }
                    else if (tile == PacmanLayout.TileType.BigPoint)
                    {
                        CreateBigPoint(new int2(x, y));
                    }
                }
            }
        }

        void CreateWall(int2 coords)
        {
            var layout = PacmanWorld.Instance.Layout;

            var wallPosition = layout.GetPositionFromCoords(coords.x, coords.y);
            var wall = Instantiate(
                _wallPrefab, transform);
            wall.transform.localPosition = (Vector2)wallPosition;
            wall.transform.localScale = (Vector2)layout.tileSize;
            _tiles[coords.x + coords.y * layout.mapDimensions.x] = wall;
        }

        void CreateSmallPoint(int2 coords)
        {
            var layout = PacmanWorld.Instance.Layout;

            var pointPosition = layout.GetPositionFromCoords(coords.x, coords.y);
            var point = Instantiate(
                _pointPrefab, transform);
            point.transform.localPosition = (Vector2)pointPosition;
            _tiles[coords.x + coords.y * layout.mapDimensions.x] = point;
        }

        void CreateBigPoint(int2 coords)
        {
            var layout = PacmanWorld.Instance.Layout;

            var pointPosition = layout.GetPositionFromCoords(coords.x, coords.y);
            var point = Instantiate(
                _bigPointPrefab, transform);
            point.transform.localPosition = (Vector2)pointPosition;
            _tiles[coords.x + coords.y * layout.mapDimensions.x] = point;
        }

        void HandleTileChanged(int2 coords)
        {
            var tileType = PacmanWorld.Instance.Layout.GetTileAtCoords(coords);
            var objectToChange = _tiles[coords.x + coords.y * PacmanWorld.Instance.Layout.mapDimensions.x];
            if (objectToChange != null)
                Destroy(objectToChange);
            if (tileType == PacmanLayout.TileType.Wall)
            {
                CreateWall(coords);
            }
            else if (tileType == PacmanLayout.TileType.SmallPoint)
            {
                CreateSmallPoint(coords);
            }
        }

        void HandleLayoutChanged()
        {
            BuildLayout();
        }
    }
}
