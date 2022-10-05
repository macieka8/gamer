using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleFeeder
    {
        Puzzle[] _puzzles;
        Puzzle _nextPuzzle;

        IPuzzle _savedPuzzle;
        bool _canSave = true;

        public bool CanSave => _canSave;

        public PuzzleFeeder(Puzzle[] puzzles)
        {
            _puzzles = puzzles;
            _nextPuzzle = GetRandom();
        }

        public Puzzle GetNext()
        {
            _canSave = true;
            var current = _nextPuzzle;
            _nextPuzzle = GetRandom();
            return current;
        }

        public Puzzle PeekNext()
        {
            return _nextPuzzle;
        }

        public IPuzzle SavePuzzle(IPuzzle puzzle)
        {
            _canSave = false;
            var nextPuzzle = _savedPuzzle;
            _savedPuzzle = puzzle;
            if (nextPuzzle == null)
                nextPuzzle = GetNext();
            return nextPuzzle;
        }

        public IPuzzle GetSaved()
        {
            return _savedPuzzle;
        }

        Puzzle GetRandom()
        {
            return _puzzles[Random.Range(0, _puzzles.Length)];
        }
    }
}
