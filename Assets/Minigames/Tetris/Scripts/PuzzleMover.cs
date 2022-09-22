using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleMover
    {
        const int FIND_ROTATION_POSITION_MAX_DISTANCE = 3;
        IReadonlyTetrisBoard _tetrisBoard;
        IPuzzle _activePuzzle;
        int2 _activePuzzlePosition;
        int _activePuzzleRotation;

        public IPuzzle ActivePuzzle => _activePuzzle;
        public int2 ActivePuzzlePosition => _activePuzzlePosition;
        public int ActivePuzzleRotation => _activePuzzleRotation;

        public PuzzleMover(IReadonlyTetrisBoard tetrisBoard)
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

        public bool FindPossibleRotation(out int2 foundPosition)
        {
            var offsets = _activePuzzle.GetTileOffset((_activePuzzleRotation+1) % _activePuzzle.RotationCount);
            var positionsToVisit = new Queue<int2>();
            positionsToVisit.Enqueue(_activePuzzlePosition);
            var visitedPositions = new List<int2>();

            while (positionsToVisit.Count > 0)
            {
                var currentPosition = positionsToVisit.Dequeue();
                Debug.Log(currentPosition);

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
                        if (TileDistance(ActivePuzzlePosition, testPosition) > FIND_ROTATION_POSITION_MAX_DISTANCE) continue;

                        positionsToVisit.Enqueue(testPosition);
                    }
                }
            }

            foundPosition = int2.zero;
            return false;
        }

        int TileDistance(int2 a, int2 b)
        {
            var c = a - b;
            return Mathf.Abs(c.x) + Mathf.Abs(c.y);
        }

        public void Rotate()
        {
            if (CanRotate())
            {
                _activePuzzleRotation = (_activePuzzleRotation+1) % _activePuzzle.RotationCount;
            }
            else if (FindPossibleRotation(out var foundPosition))
            {
                _activePuzzleRotation = (_activePuzzleRotation+1) % _activePuzzle.RotationCount;
                _activePuzzlePosition = foundPosition;
                Debug.Log("Found");
            }
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
