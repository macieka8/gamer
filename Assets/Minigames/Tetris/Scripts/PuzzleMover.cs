using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleMover
    {
        const int FIND_ROTATION_POSITION_MAX_DISTANCE = 3;
        IReadonlyTetrisBoard _tetrisBoard;
        ActivePuzzle _activePuzzle;
        bool _isOnGround;

        public ActivePuzzle ActivePuzzle => _activePuzzle;
        public bool IsOnGround => _isOnGround;

        public PuzzleMover(IReadonlyTetrisBoard tetrisBoard)
        {
            _tetrisBoard = tetrisBoard;
        }

        public void SetActivePuzzle(IPuzzle puzzleData, int2 position, int rotation = 0)
        {
            _activePuzzle = new ActivePuzzle(puzzleData, position, rotation);
            _isOnGround = false;
        }

        public bool CanRotate()
        {
            var offsets = _activePuzzle.GetNextRotationTilesOffset();
            foreach (var offset in offsets)
            {
                var tilePosition = offset + _activePuzzle.Position;
                if (!_tetrisBoard.IsPositionOnBoard(tilePosition)
                    || !Tile.IsNullOrEmpty(_tetrisBoard.GetValue(tilePosition)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool TryRotate(out int2 foundPosition)
        {
            var offsets = _activePuzzle.GetNextRotationTilesOffset();
            var positionsToVisit = new Queue<int2>();
            positionsToVisit.Enqueue(_activePuzzle.Position);
            var visitedPositions = new List<int2>();

            while (positionsToVisit.Count > 0)
            {
                var currentPosition = positionsToVisit.Dequeue();

                visitedPositions.Add(currentPosition);
                if (_tetrisBoard.FitsOnBoard(offsets, currentPosition))
                {
                    foundPosition = currentPosition;
                    return true;
                }

                for (int y = 0; y >= -1; y--)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        if (y == 0 && x == 0) continue;
                        var testPosition = new int2(currentPosition.x + x, currentPosition.y + y);
                        if (!_tetrisBoard.IsPositionOnBoard(testPosition)) continue;
                        if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(testPosition))) continue;
                        if (visitedPositions.Contains(testPosition)) continue;
                        if (positionsToVisit.Contains(testPosition)) continue;
                        if (Tile.Distance(_activePuzzle.Position, testPosition) > FIND_ROTATION_POSITION_MAX_DISTANCE) continue;

                        positionsToVisit.Enqueue(testPosition);
                    }
                }
            }
            foundPosition = int2.zero;
            return false;
        }

        public void Rotate()
        {
            if (CanRotate())
            {
                _activePuzzle.Rotate();
            }
            else if (TryRotate(out var foundPosition))
            {
                _activePuzzle.Position = foundPosition;
                _activePuzzle.Rotate();
            }
            _isOnGround = !CanMoveDown();
        }

        public bool CanMoveLeft()
        {
            foreach (var tileOffset in _activePuzzle.GetTilesOffset())
            {
                var newBoardSpaceTilePosition = _activePuzzle.Position + tileOffset - new int2(1, 0);

                if (newBoardSpaceTilePosition.x < 0) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveRight()
        {
            foreach (var tileOffset in _activePuzzle.GetTilesOffset())
            {
                var newBoardSpaceTilePosition = _activePuzzle.Position + tileOffset + new int2(1, 0);

                if (newBoardSpaceTilePosition.x >= TetrisBoard.Width) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public bool CanMoveDown()
        {
            foreach (var tileOffset in _activePuzzle.GetTilesOffset())
            {
                var newBoardSpaceTilePosition = _activePuzzle.Position + tileOffset + new int2(0, 1);

                if (newBoardSpaceTilePosition.y >= TetrisBoard.Height) return false;
                if (!Tile.IsNullOrEmpty(_tetrisBoard.GetValue(newBoardSpaceTilePosition))) return false;
            }
            return true;
        }

        public void MoveLeft()
        {
            if (CanMoveLeft())
            {
                _activePuzzle.Position += new int2(-1, 0);
            }
            _isOnGround = !CanMoveDown();
        }

        public void MoveRight()
        {
            if (CanMoveRight())
            {
                _activePuzzle.Position += new int2(1, 0);
            }
            _isOnGround = !CanMoveDown();
        }

        public void MoveDown()
        {
            if (CanMoveDown())
            {
                _activePuzzle.Position += new int2(0, 1);
            }
            _isOnGround = !CanMoveDown();
        }

        public void HardDropDown()
        {
            while(CanMoveDown())
            {
                _activePuzzle.Position += new int2(0, 1);
            }
            _isOnGround = true;
        }

        public PuzzleMover Clone()
        {
            var puzzleMover = new PuzzleMover(_tetrisBoard);
            puzzleMover.SetActivePuzzle(
                _activePuzzle.PuzzleData, _activePuzzle.Position, _activePuzzle.Rotation);
            puzzleMover._isOnGround = _isOnGround;
            return puzzleMover;
        }
    }
}
