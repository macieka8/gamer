using Unity.Mathematics;

namespace gamer.tetris
{
    public class PuzzleMover
    {
        TetrisBoard _tetrisBoard;
        IPuzzle _activePuzzle;
        int2 _activePuzzlePosition;

        public IPuzzle ActivePuzzle => _activePuzzle;
        public int2 ActivePuzzlePosition => _activePuzzlePosition;

        public PuzzleMover(TetrisBoard tetrisBoard)
        {
            _tetrisBoard = tetrisBoard;
        }

        public void SetActivePuzzle(IPuzzle puzzle, int2 position)
        {
            _activePuzzle = puzzle;
            _activePuzzlePosition = position;
        }

        public bool CanMoveLeft()
        {
            foreach (var tileOffset in _activePuzzle.TilesOffset)
            {
                var newBoardSpaceTilePosition = _activePuzzlePosition + tileOffset - new int2(1, 0);

                if (newBoardSpaceTilePosition.x < 0) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveRight()
        {
            foreach (var tileOffset in _activePuzzle.TilesOffset)
            {
                var newBoardSpaceTilePosition = _activePuzzlePosition + tileOffset + new int2(1, 0);

                if (newBoardSpaceTilePosition.x >= TetrisBoard.Width) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveDown()
        {
            foreach (var tileOffset in _activePuzzle.TilesOffset)
            {
                var newBoardSpaceTilePosition = _activePuzzlePosition + tileOffset + new int2(0, 1);

                if (newBoardSpaceTilePosition.y >= TetrisBoard.Height) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public void MoveLeft()
        {
            _activePuzzlePosition.x--;
        }

        public void MoveRight()
        {
            _activePuzzlePosition.x++;
        }

        public void MoveDown()
        {
            _activePuzzlePosition.y++;
        }
    }
}
