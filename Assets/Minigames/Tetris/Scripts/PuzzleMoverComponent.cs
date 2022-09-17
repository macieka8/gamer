using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

namespace gamer.tetris
{
    public class PuzzleMoverComponent : MonoBehaviour
    {
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] InputActionReference _rotateInputAction;
        [SerializeField] TetrisBoardComponent _tetrisBoard;

        [SerializeField] Puzzle _testPuzzle;
        [SerializeField] int2 _testPosition;

        PuzzleMover _puzzleMover;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void Start()
        {
            _puzzleMover = new PuzzleMover(_tetrisBoard.Board);
            _puzzleMover.SetActivePuzzle(_testPuzzle, _testPosition);

            _moveInputAction.action.performed += HandleMoveInput;
            _rotateInputAction.action.performed += HandleRotateInput;
            //todo: remove enable
            _moveInputAction.action.Enable();
            _rotateInputAction.action.Enable();
        }

        void OnDestroy()
        {
            _moveInputAction.action.performed -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;
        }

        void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            var inputValue = ctx.ReadValue<float>();
            if (inputValue < 0f)
            {
                if (_puzzleMover.CanMoveLeft()) _puzzleMover.MoveLeft();
            }
            else if (inputValue > 0f)
            {
                if (_puzzleMover.CanMoveRight()) _puzzleMover.MoveRight();
            }
        }

        void HandleRotateInput(InputAction.CallbackContext ctx)
        {

        }
    }
}
