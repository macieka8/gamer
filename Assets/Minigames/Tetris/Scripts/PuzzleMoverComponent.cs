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
        bool _isTryingToPlacePuzzle;
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
            if (_timeLeftToMove <= 0f)
            {
                Move(_direction);
                _timeLeftToMove = _moveTime;
            }
            _timeLeftToMove -= Time.deltaTime;

            if (_isTryingToPlacePuzzle)
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
        }

        void OnDestroy()
        {
            UpdateSystemComponent.OnUpdate -= HandleUpdate;
            _moveInputAction.action.performed -= HandleMoveInput;
            _moveInputAction.action.canceled -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;
        }

        void Move(float direction)
        {
            if (direction < 0f)
            {
                if (_puzzleMover.CanMoveLeft())
                {
                    _puzzleMover.MoveLeft();
                    _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
                    _currentPlacingCountLimit ++;
                }
            }
            else if (direction > 0f)
            {
                if (_puzzleMover.CanMoveRight())
                {
                    _puzzleMover.MoveRight();
                    _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
                    _currentPlacingCountLimit ++;
                }
            }
        }

        void HandleUpdate()
        {
            if (_puzzleMover.CanMoveDown())
            {
                _puzzleMover.MoveDown();
                _isTryingToPlacePuzzle = false;
                _currentPlacingCountLimit = 0;
            }
            else
            {
                _isTryingToPlacePuzzle = true;
            }
        }

        void PlacePuzzleOnBoard()
        {
            _tetrisBoard.SetValue(_puzzleMover.ActivePuzzlePosition, _puzzleMover.ActivePuzzle, _puzzleMover.ActivePuzzleRotation);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);
            _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
            _isTryingToPlacePuzzle = false;
            _currentPlacingCountLimit = 0;
        }

        void HandleMoveInput(InputAction.CallbackContext ctx)
        {
            _direction = ctx.ReadValue<float>();
            _timeLeftToMove = _moveTime * 4;
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
            _isTryingToPlacePuzzle = true;
        }
    }
}
