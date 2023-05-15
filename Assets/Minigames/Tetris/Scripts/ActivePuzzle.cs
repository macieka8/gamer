using System.Collections.Generic;
using Unity.Mathematics;

namespace gamer.tetris
{
    public class ActivePuzzle
    {
        IPuzzle _puzzleData;
        int2 _position;
        int _rotation;
        int2[] _tilesOffsets;

        public IPuzzle PuzzleData => _puzzleData;
        public int2 Position {
            get => _position;
            set => _position = value;
        }
        public  int Rotation => _rotation;

        public ActivePuzzle(IPuzzle puzzleData, int2 position, int rotation = 0)
        {
            _puzzleData = puzzleData;
            _position = position;
            _rotation = rotation;
            _tilesOffsets = _puzzleData.GetTileOffset(rotation);
        }

        public int2[] GetTilesOffset()
        {
            return _tilesOffsets;
        }

        public int2[] GetNextRotationTilesOffset()
        {
            return _puzzleData.GetTileOffset((_rotation + 1) % _puzzleData.RotationCount);
        }

        public void Rotate()
        {
            _rotation = (_rotation + 1) % _puzzleData.RotationCount;
            _tilesOffsets = _puzzleData.GetTileOffset(_rotation);
        }

        public ActivePuzzle Clone()
        {
            return new ActivePuzzle(_puzzleData, _position, _rotation);
        }
    }
}
