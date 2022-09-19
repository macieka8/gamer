using UnityEngine;

namespace gamer.tetris
{

    public class PuzzleFeederComponent : MonoBehaviour
    {
        [SerializeField] Puzzle[] _allPuzzles;

        PuzzleFeeder _puzzleFeeder;

        void Awake()
        {
            _puzzleFeeder = new PuzzleFeeder(_allPuzzles);
        }

        public Puzzle GetRandom()
        {
            return _puzzleFeeder.GetRandom();
        }
    }
}
