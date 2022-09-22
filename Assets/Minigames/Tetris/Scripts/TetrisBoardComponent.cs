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

        void Start()
        {
            for (int i = 0; i < _initalBoard.Length; i++)
            {
                int x = i % TetrisBoard.Width;
                int y = i / TetrisBoard.Width;

                _tetrisBoard.SetValue(x, y, _initalBoard[i]);
            }
            OnValueSet += HandleValueSet;
        }

        void OnDestroy()
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
            _tetrisBoard.SetValue(x, y , value);
            OnValueSet?.Invoke();
        }

        public void SetValue(int2 position, Tile value)
        {
            _tetrisBoard.SetValue(position, value);
            OnValueSet?.Invoke();
        }

        public void SetValue(int2 position, IPuzzle puzzle, int rotation)
        {
            _tetrisBoard.SetValue(position, puzzle, rotation);
            OnValueSet?.Invoke();
        }

        public bool FitsOnBoard(int2[] tilesOffsets, int2 position)
        {
            return _tetrisBoard.FitsOnBoard(tilesOffsets, position);
        }
    }
}
