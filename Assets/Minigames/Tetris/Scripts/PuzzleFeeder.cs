using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleFeeder
    {
        Puzzle[] _puzzles;
        Puzzle _nextPuzzle;

        public PuzzleFeeder(Puzzle[] puzzles)
        {
            _puzzles = puzzles;
            _nextPuzzle = GetRandom();
        }

        public Puzzle GetNext()
        {
            var current = _nextPuzzle;
            _nextPuzzle = GetRandom();
            return current;
        }

        public Puzzle PeekNext()
        {
            return _nextPuzzle;
        }

        Puzzle GetRandom()
        {
            return _puzzles[Random.Range(0, _puzzles.Length)];
        }
    }
}
