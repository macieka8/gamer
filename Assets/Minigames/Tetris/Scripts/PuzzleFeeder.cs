using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleFeeder
    {
        Puzzle[] _puzzles;

        public PuzzleFeeder(Puzzle[] puzzles)
        {
            _puzzles = puzzles;
        }

        public Puzzle GetRandom()
        {
            return _puzzles[Random.Range(0, _puzzles.Length)];
        }
    }
}
