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
        [SerializeField] float _moveTime;
        [SerializeField] float _timeBeforePlacingPuzzle;
        [SerializeField] int _moveCountLimitBeforePlacingPuzzle;

        PuzzleMover _puzzleMover;
        float _timeLeftToMove;
        float _direction;
        int _currentPlacingCountLimit;
        float _timeLeftToPlacePuzzle;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void Start()
        {
            _timeLeftToMove = _moveTime;

            _puzzleMover = new PuzzleMover(_tetrisBoard.ReadonlyBoard);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);

            UpdateSystemComponent.OnUpdate += HandleUpdate;
            //todo: remove enable
            _moveInputAction.action.performed += HandleMoveInput;
            _moveInputAction.action.canceled += HandleMoveInput;
            _moveInputAction.action.Enable();
            _rotateInputAction.action.performed += HandleRotateInput;
            _rotateInputAction.action.Enable();
        }

        void Update()
        {
            UpdateMove();
            UpdatePuzzlePlacing();
        }

        void OnDestroy()
        {
            UpdateSystemComponent.OnUpdate -= HandleUpdate;
            _moveInputAction.action.performed -= HandleMoveInput;
            _moveInputAction.action.canceled -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;
        }

        void UpdateMove()
        {
            if (_timeLeftToMove <= 0f)
            {
                Move(_direction);
                _timeLeftToMove = _moveTime;
            }
            _timeLeftToMove -= Time.deltaTime;
        }

        void UpdatePuzzlePlacing()
        {
            if (_puzzleMover.IsOnGround)
            {
                if (_timeLeftToPlacePuzzle <= 0f
                || _currentPlacingCountLimit >= _moveCountLimitBeforePlacingPuzzle)
                {
                    PlacePuzzleOnBoard();
                }
                else
                {
                    _timeLeftToPlacePuzzle -= Time.deltaTime;
                }
            }
            else
            {
                _currentPlacingCountLimit = 0;
            }
        }

        void Move(float direction)
        {
            if (direction < 0f)
            {
                _puzzleMover.MoveLeft();
                _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
                _currentPlacingCountLimit++;
            }
            else if (direction > 0f)
            {
                _puzzleMover.MoveRight();
                _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
                _currentPlacingCountLimit++;
            }
        }

        void HandleUpdate()
        {
            _puzzleMover.MoveDown();
        }

        void PlacePuzzleOnBoard()
        {
            _tetrisBoard.SetValue(_puzzleMover.ActivePuzzle);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);
            _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
            _currentPlacingCountLimit = 0;
        }

        void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            _direction = ctx.ReadValue<float>();
            _timeLeftToMove = _moveTime * 4f;
            Move(_direction);
        }

        void HandleRotateInput(InputAction.CallbackContext ctx)
        {
            _puzzleMover.Rotate();
        }

        public void HardDropDown()
        {
            _puzzleMover.HardDropDown();
            _timeLeftToPlacePuzzle = 0f;
        }
    }
}
