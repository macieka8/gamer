using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    public class TetrisBoardComponent : MonoBehaviour
    {
        [SerializeField] Tile[] _initalBoard;
        TetrisBoard _tetrisBoard = new TetrisBoard();

        public event System.Action OnValueSet;
        public IReadonlyTetrisBoard ReadonlyBoard => _tetrisBoard;

        void OnEnable()
        {
            _tetrisBoard.Clear();
            for (int i = 0; i < _initalBoard.Length; i++)
            {
                int x = i % TetrisBoard.Width;
                int y = i / TetrisBoard.Width;

                _tetrisBoard.SetValue(x, y, _initalBoard[i]);
            }
            OnValueSet += HandleValueSet;
        }

        void OnDisable()
        {
            OnValueSet -= HandleValueSet;
        }

        void HandleValueSet()
        {
            var filledRows = _tetrisBoard.GetFilledRows();
            for (int i = 0; i < filledRows.Length; i++)
            {
                _tetrisBoard.DestroyRow(filledRows[i]);
            }
        }

        public Tile GetValue(int x, int y)
        {
            return _tetrisBoard.GetValue(x, y);
        }

        public Tile GetValue(int2 position)
        {
            return _tetrisBoard.GetValue(position);
        }

        public void SetValue(int x , int y, Tile value)
        {
            _tetrisBoard.SetValue(x, y , value, true);
            OnValueSet?.Invoke();
        }

        public void SetValue(int2 position, Tile value)
        {
            _tetrisBoard.SetValue(position, value, true);
            OnValueSet?.Invoke();
        }

        public void SetValue(IPuzzle puzzle, int2 position, int rotation)
        {
            _tetrisBoard.SetValue(puzzle, position, rotation, true);
            OnValueSet?.Invoke();
        }

        public void SetValue(ActivePuzzle activePuzzle)
        {
            _tetrisBoard.SetValue(activePuzzle.PuzzleData, activePuzzle.Position, activePuzzle.Rotation, true);
            OnValueSet?.Invoke();
        }

        public bool FitsOnBoard(int2[] tilesOffsets, int2 position)
        {
            return _tetrisBoard.FitsOnBoard(tilesOffsets, position);
        }
    }
}
