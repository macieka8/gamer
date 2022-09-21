using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleMover
    {
        TetrisBoard _tetrisBoard;
        IPuzzle _activePuzzle;
        int2 _activePuzzlePosition;
        int _activePuzzleRotation;

        public IPuzzle ActivePuzzle => _activePuzzle;
        public int2 ActivePuzzlePosition => _activePuzzlePosition;
        public int ActivePuzzleRotation => _activePuzzleRotation;

        public PuzzleMover(TetrisBoard tetrisBoard)
        {
            _tetrisBoard = tetrisBoard;
        }

        public void SetActivePuzzle(IPuzzle puzzle, int2 position)
        {
            _activePuzzle = puzzle;
            _activePuzzlePosition = position;
        }

        public int2[] GetTilesOffset()
        {
            return _activePuzzle.GetTileOffset(_activePuzzleRotation);
        }

        public bool CanRotate()
        {
            var offsets = _activePuzzle.GetTileOffset((_activePuzzleRotation+1) % _activePuzzle.RotationCount);
            foreach (var offset in offsets)
            {
                var position = offset + _activePuzzlePosition;
                if (!_tetrisBoard.IsPositionOnBoard(position)
                    || !Tile.IsNullOrEmpty(_tetrisBoard.GetValue(position)))
                {
                    return false;
                }
            }
            return true;
        }

        public void Rotate()
        {
            if (!CanRotate()) return;
            _activePuzzleRotation = (_activePuzzleRotation+1) % _activePuzzle.RotationCount;
        }

        public bool CanMoveLeft()
        {
            foreach (var tileOffset in _activePuzzle.GetTileOffset(_activePuzzleRotation))
            {
                var newBoardSpaceTilePosition = _activePuzzlePosition + tileOffset - new int2(1, 0);

                if (newBoardSpaceTilePosition.x < 0) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveRight()
        {
            foreach (var tileOffset in _activePuzzle.GetTileOffset(_activePuzzleRotation))
            {
                var newBoardSpaceTilePosition = _activePuzzlePosition + tileOffset + new int2(1, 0);

                if (newBoardSpaceTilePosition.x >= TetrisBoard.Width) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveDown()
        {
            foreach (var tileOffset in _activePuzzle.GetTileOffset(_activePuzzleRotation))
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

        public void HardDropDown()
        {
            while(CanMoveDown())
            {
                MoveDown();
            }
        }
    }
}
