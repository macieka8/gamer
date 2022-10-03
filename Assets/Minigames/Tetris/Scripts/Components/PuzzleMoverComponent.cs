using UnityEngine;
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
        [SerializeField] FloatInputSender _movementInput;
        [SerializeField] EmptyInputSender _rotationInput;
        [SerializeField] BoolInputSender _softDropInput;
        [SerializeField] EmptyInputSender _hardDropInput;
        [SerializeField] EmptyInputSender _savePuzzleInput;

        DropState _dropState;
        float _timeLeftToMove;
        float _direction;
        int _currentPlacingCountLimit;
        float _timeLeftToPlacePuzzle;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void OnEnable()
        {
            UpdateSystemComponent.Instance.OnUpdate += HandleUpdate;
            _movementInput.OnInput += HandleMoveInput;
            _rotationInput.OnInput += HandleRotateInput;
            _softDropInput.OnInput += HandleSoftDropInput;
            _hardDropInput.OnInput += HandleHardDrop;
            _savePuzzleInput.OnInput += HandlePuzzleSave;
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
            _movementInput.OnInput -= HandleMoveInput;
            _rotationInput.OnInput -= HandleRotateInput;
            _softDropInput.OnInput -= HandleSoftDropInput;
            _hardDropInput.OnInput -= HandleHardDrop;
            _savePuzzleInput.OnInput -= HandlePuzzleSave;
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

        void HandleMoveInput(float direction)
        {
            _direction = direction;
            _timeLeftToMove = _moveTime * 4f;
            Move(_direction);
        }

        void HandleRotateInput(object obj)
        {
            _puzzleMover.Rotate();
            _timeLeftToPlacePuzzle = _timeBeforePlacingPuzzle;
            _currentPlacingCountLimit++;
        }

        void HandleSoftDropInput(bool isSoftDropCanceled)
        {
            if (!isSoftDropCanceled)
            {
                if (_dropState != DropState.Normal) return;

                _dropState = DropState.SoftDrop;
                UpdateSystemComponent.Instance.SetUpdateTime(_softDropFallTime);
            }
            else
            {
                if (_dropState != DropState.SoftDrop) return;

                _dropState = DropState.Normal;
                UpdateSystemComponent.Instance.SetDefaultUpdateTime();
            }
        }

        void HandleHardDrop(object obj)
        {
            _puzzleMover.HardDropDown();
            _timeLeftToPlacePuzzle = 0f;
            UpdateSystemComponent.Instance.ForceUpdate();
        }

        void HandlePuzzleSave(object obj)
        {
            _puzzleMover.SetActivePuzzle(
                _puzzleFeeder.SavePuzzle(_puzzleMover.ActivePuzzle.PuzzleData), _spawnPosition);
        }
    }
}
