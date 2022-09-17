using UnityEngine;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class PuzzleMoverDisplayer : MonoBehaviour
    {
        [SerializeField] GameObject _tileObject;
        [SerializeField] PuzzleMoverComponent _puzzleMover;

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
            var startPosition = new Vector3(
                -(TetrisBoard.Width / 2f) + 0.5f, (TetrisBoard.Height / 2f) - 0.5f, 0f);
            var puzzleMover = _puzzleMover.PuzzleMover;
            for (int i = 0; i < puzzleMover.ActivePuzzle.TilesCount; i++)
            {
                var boardSpaceTilePosition =
                    puzzleMover.ActivePuzzlePosition + puzzleMover.ActivePuzzle.TilesOffset[i];
                var puzzle = Instantiate(_tileObject, transform);
                puzzle.transform.localPosition =
                    new Vector3(
                        startPosition.x + boardSpaceTilePosition.x,
                        startPosition.y - boardSpaceTilePosition.y,
                        0f);
                var sprite = puzzleMover.ActivePuzzle.Tiles[i].Sprite;
                puzzle.GetComponent<SpriteRenderer>().sprite = sprite;
                _tiles.Add(puzzle);
            }
        }
    }
}
