using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;

namespace gamer.tetris
{
    public enum DropState
    {
        Normal,
        SoftDrop,
    }
    public class PuzzleMoverComponent : MonoBehaviour
    {
        [Header("Player Inputs")]
        [SerializeField] InputActionReference _moveInputAction;
        [SerializeField] InputActionReference _rotateInputAction;
        [SerializeField] InputActionReference _softDropInputAction;
        [SerializeField] InputActionReference _hardDropInputAction;

        [Header("References")]
        [SerializeField] TetrisBoardComponent _tetrisBoard;
        [SerializeField] PuzzleFeederComponent _puzzleFeeder;


        [Header("Puzzle Movement Settings")]
        [SerializeField] float _moveTime;
        [SerializeField] float _timeBeforePlacingPuzzle;
        [SerializeField] int _moveCountLimitBeforePlacingPuzzle;
        [SerializeField] int2 _spawnPosition;

        [Header("Puzzle Drop Settings")]
        [SerializeField] float _softDropFallTime;

        PuzzleMover _puzzleMover;
        DropState _dropState;
        float _timeLeftToMove;
        float _direction;
        int _currentPlacingCountLimit;
        float _timeLeftToPlacePuzzle;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void OnEnable()
        {
            UpdateSystemComponent.Instance.OnUpdate += HandleUpdate;
            //todo: remove enable
            _moveInputAction.action.performed += HandleMoveInput;
            _moveInputAction.action.canceled += HandleMoveInput;
            _moveInputAction.action.Enable();

            _rotateInputAction.action.performed += HandleRotateInput;
            _rotateInputAction.action.Enable();

            _softDropInputAction.action.started += HandleSoftDropStarted;
            _softDropInputAction.action.canceled += HandleSoftDropCanceled;
            _softDropInputAction.action.Enable();

            _hardDropInputAction.action.performed += HandleHardDrop;
            _hardDropInputAction.action.Enable();
        }

        void Start()
        {
            _timeLeftToMove = _moveTime;

            _puzzleMover = new PuzzleMover(_tetrisBoard.ReadonlyBoard);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);
        }

        void Update()
        {
            UpdateMove();
            UpdatePuzzlePlacing();
        }

        void OnDisable()
        {
            UpdateSystemComponent.Instance.OnUpdate -= HandleUpdate;
            _moveInputAction.action.performed -= HandleMoveInput;
            _moveInputAction.action.canceled -= HandleMoveInput;
            _rotateInputAction.action.performed -= HandleRotateInput;

            _softDropInputAction.action.started -= HandleSoftDropStarted;
            _softDropInputAction.action.canceled -= HandleSoftDropStarted;
            _hardDropInputAction.action.performed -= HandleHardDrop;
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

        void HandleSoftDropStarted(InputAction.CallbackContext ctx)
        {
            if (_dropState != DropState.Normal) return;

            _dropState = DropState.SoftDrop;
            UpdateSystemComponent.Instance.SetUpdateTime(_softDropFallTime);
        }

        void HandleSoftDropCanceled(InputAction.CallbackContext ctx)
        {
            if (_dropState != DropState.SoftDrop) return;

            _dropState = DropState.Normal;
            UpdateSystemComponent.Instance.SetDefaultUpdateTime();
        }

        void HandleHardDrop(InputAction.CallbackContext ctx)
        {
            _puzzleMover.HardDropDown();
            _timeLeftToPlacePuzzle = 0f;
            UpdateSystemComponent.Instance.ForceUpdate();
        }
    }
}
