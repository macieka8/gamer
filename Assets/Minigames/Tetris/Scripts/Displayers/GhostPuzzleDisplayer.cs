using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace gamer.tetris
{
    public class GhostPuzzleDisplayer : MonoBehaviour
    {
        [SerializeField] GameObject _tileObject;
        [SerializeField] Color _color;
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
            if (_puzzleMover.PuzzleMover.ActivePuzzle == null) return;
            var ghostPuzzleMover = _puzzleMover.PuzzleMover.Clone();
            ghostPuzzleMover.HardDropDown();
            var ghostPuzzle = ghostPuzzleMover.ActivePuzzle;

            var startPosition = new Vector3(
                -(TetrisBoard.Width / 2f) + 0.5f, (TetrisBoard.Height / 2f) - 0.5f, 0f);
            var puzzleOffsets = ghostPuzzle.GetTilesOffset();
            for (int i = 0; i < ghostPuzzle.PuzzleData.TilesCount; i++)
            {
                var boardSpaceTilePosition = ghostPuzzle.Position + puzzleOffsets[i];
                var puzzle = Instantiate(_tileObject, transform);
                puzzle.transform.localPosition =
                    new Vector3(
                        startPosition.x + boardSpaceTilePosition.x,
                        startPosition.y - boardSpaceTilePosition.y,
                        0f);
                var spriteRenderer = puzzle.GetComponent<SpriteRenderer>();
                spriteRenderer.color = _color;
                _tiles.Add(puzzle);
            }
        }
    }
}
