using UnityEngine;

namespace gamer.tetris
{
    public class GameEnder : MonoBehaviour
    {
        [SerializeField] TetrisBoardComponent _board;
        [SerializeField] GameController _gameController;

        void OnEnable()
        {
            _board.ReadonlyBoard.OnTileOverridden += HandleTileOverriden;
        }

        void OnDisable()
        {
            _board.ReadonlyBoard.OnTileOverridden -= HandleTileOverriden;
        }

        void HandleTileOverriden()
        {
            _gameController.StartMenu();
        }
    }
}
