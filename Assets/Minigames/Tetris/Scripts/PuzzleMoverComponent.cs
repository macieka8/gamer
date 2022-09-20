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
        [SerializeField] PuzzleFeederComponent _puzzleFeeder;

        [SerializeField] int2 _spawnPosition;

        PuzzleMover _puzzleMover;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void Start()
        {
            _puzzleMover = new PuzzleMover(_tetrisBoard.Board);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);

            UpdateSystemComponent.OnUpdate += HandleUpdate;

            _moveInputAction.action.performed += HandleMoveInput;
            _rotateInputAction.action.performed += HandleRotateInput;
            //todo: remove enable
            _moveInputAction.action.Enable();
            _rotateInputAction.action.Enable();
        }

        void OnDestroy()
        {
            UpdateSystemComponent.OnUpdate -= HandleUpdate;
            _moveInputAction.action.performed -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;
        }

        void HandleUpdate()
        {
            if (_puzzleMover.CanMoveDown())
            {
                _puzzleMover.MoveDown();
            }
            else
            {
                _tetrisBoard.Board.SetValue(_puzzleMover.ActivePuzzlePosition, _puzzleMover.ActivePuzzle, _puzzleMover.ActivePuzzleRotation);
                _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);
            }
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
            _puzzleMover.Rotate();
        }
    }
}
