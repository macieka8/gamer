using UnityEngine;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class TetrisBoardDisplayer : MonoBehaviour
    {
        [SerializeField] GameObject _tileObject;
        [SerializeField] TetrisBoardComponent _tetrisBoard;

        List<GameObject> _tiles = new List<GameObject>();

        void Update()
        {
            ClearTiles();
            DrawTiles();
        }

        void ClearTiles()
        {
            foreach(var puzzle in _tiles)
            {
                Destroy(puzzle);
            }
            _tiles.Clear();
        }

        void DrawTiles()
        {
            var startPosition = new Vector3(-(TetrisBoard.Width / 2f) + 0.5f, (TetrisBoard.Height / 2f) - 0.5f, 0f);
            for (int i = 0; i < TetrisBoard.Size; i++)
            {
                int x = i % TetrisBoard.Width;
                int y = i / TetrisBoard.Width;
                var currentTile = _tetrisBoard.GetValue(x, y);
                if (!Tile.IsNullOrEmpty(currentTile))
                {
                    var puzzle = Instantiate(_tileObject, transform);
                    puzzle.transform.localPosition =
                        new Vector3(startPosition.x + x, startPosition.y - y, 0f);
                    var spriteRenderer = puzzle.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = currentTile.Sprite;
                    spriteRenderer.color = currentTile.Color;
                    _tiles.Add(puzzle);
                }
            }
        }
    }
}
