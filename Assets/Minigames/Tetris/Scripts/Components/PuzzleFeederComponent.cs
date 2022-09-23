using UnityEngine;
using System.Collections.Generic;

namespace gamer.tetris
{
    public class PuzzleFeederComponent : MonoBehaviour
    {
        [SerializeField] Puzzle[] _allPuzzles;

        PuzzleFeeder _puzzleFeeder;

        public event System.Action OnNextPuzzle;

        void Awake()
        {
            _puzzleFeeder = new PuzzleFeeder(_allPuzzles);
        }

        public Puzzle GetNext()
        {
            var puzzle = _puzzleFeeder.GetNext();
            OnNextPuzzle?.Invoke();
            return puzzle;
        }

        public Puzzle PeekNext()
        {
            return _puzzleFeeder.PeekNext();
        }
    }
}
