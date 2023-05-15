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
            if (_puzzleMover.PuzzleMover.ActivePuzzle == null) return;
            var startPosition = new Vector3(
                -(TetrisBoard.Width / 2f) + 0.5f, (TetrisBoard.Height / 2f) - 0.5f, 0f);
            var activePuzzle = _puzzleMover.PuzzleMover.ActivePuzzle;
            var puzzleOffsets = activePuzzle.GetTilesOffset();
            for (int i = 0; i < activePuzzle.PuzzleData.TilesCount; i++)
            {
                var boardSpaceTilePosition = activePuzzle.Position + puzzleOffsets[i];
                var puzzle = Instantiate(_tileObject, transform);
                puzzle.transform.localPosition =
                    new Vector3(
                        startPosition.x + boardSpaceTilePosition.x,
                        startPosition.y - boardSpaceTilePosition.y,
                        0f);
                var spriteRenderer = puzzle.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = activePuzzle.PuzzleData.Tiles[i].Sprite;
                spriteRenderer.color = activePuzzle.PuzzleData.Tiles[i].Color;
                _tiles.Add(puzzle);
            }
        }
    }
}
