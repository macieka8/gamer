using UnityEngine;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class NextPuzzleDisplayer : MonoBehaviour
    {
        [SerializeField] GameObject _tileObject;
        [SerializeField] PuzzleFeederComponent _puzzleFeeder;

        List<GameObject> _tiles = new List<GameObject>();

        void Start()
        {
            _puzzleFeeder.OnNextPuzzle += HandleNextPuzzle;
        }

        void OnDestroy()
        {
            _puzzleFeeder.OnNextPuzzle -= HandleNextPuzzle;
        }

        void HandleNextPuzzle()
        {
            ClearTiles();
            DrawPuzzle();
        }

        void ClearTiles()
        {
            foreach(var puzzle in _tiles)
            {
                Destroy(puzzle);
            }
            _tiles.Clear();
        }

        void DrawPuzzle()
        {
            var puzzleToDraw = _puzzleFeeder.PeekNext();
            var tileOffsets = puzzleToDraw.GetTileOffset(0);
            for (int i = 0; i < puzzleToDraw.TilesCount; i++)
            {
                var puzzle = Instantiate(_tileObject, transform);
                puzzle.transform.localPosition =
                    new Vector3(tileOffsets[i].x, - tileOffsets[i].y, 0f);
                var sprite = puzzleToDraw.Tiles[i].Sprite;
                puzzle.GetComponent<SpriteRenderer>().sprite = sprite;
                _tiles.Add(puzzle);
            }
        }
    }
}
