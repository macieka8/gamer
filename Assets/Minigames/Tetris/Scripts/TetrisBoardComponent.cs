using UnityEngine;

namespace gamer.tetris
{
    public class TetrisBoardComponent : MonoBehaviour
    {
        [SerializeField] Tile[] _initalBoard;
        TetrisBoard _tetrisBoard = new TetrisBoard();

        public TetrisBoard Board => _tetrisBoard;

        void Start()
        {
            for (int i = 0; i < _initalBoard.Length; i++)
            {
                int x = i % TetrisBoard.Width;
                int y = i / TetrisBoard.Width;

                _tetrisBoard.SetValue(x, y, _initalBoard[i]);
            }
        }
    }
}
