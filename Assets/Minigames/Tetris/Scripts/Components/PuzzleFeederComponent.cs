using UnityEngine;

namespace gamer.tetris
{
    public class PuzzleFeederComponent : MonoBehaviour
    {
        [SerializeField] Puzzle[] _allPuzzles;

        PuzzleFeeder _puzzleFeeder;

        public event System.Action OnNextPuzzle;

        void OnEnable()
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

        public IPuzzle SavePuzzle(IPuzzle puzzle)
        {
            if (!_puzzleFeeder.CanSave) return null;
            var nextPuzzle = _puzzleFeeder.SavePuzzle(puzzle);
            OnNextPuzzle?.Invoke();
            return nextPuzzle;
        }

        public IPuzzle GetSaved()
        {
            return _puzzleFeeder.GetSaved();
        }
    }
}
