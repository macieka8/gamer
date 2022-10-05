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
        IPuzzleMoverInput _input;

        DropState _dropState;
        float _timeLeftToMove;
        float _direction;
        int _currentPlacingCountLimit;
        float _timeLeftToPlacePuzzle;

        public PuzzleMover PuzzleMover => _puzzleMover;

        void Awake()
        {
            _input = GetComponent<IPuzzleMoverInput>();
        }

        void OnEnable()
        {
            UpdateSystemComponent.Instance.OnUpdate += HandleUpdate;
            _input.OnMovementInput += HandleMoveInput;
            _input.OnRotationInput += HandleRotateInput;
            _input.OnSoftDropInput += HandleSoftDropInput;
            _input.OnHardDropInput += HandleHardDrop;
            _input.OnSavePuzzleInput += HandlePuzzleSave;
        }

        void Start()
        {
            _timeLeftToMove = _moveTime;

            _puzzleMover = new PuzzleMover(_tetrisBoard.ReadonlyBoard);
            _puzzleMover.SetActivePuzzle(_puzzleFeeder.GetNext(), _spawnPosition);
        }

        void Update()
        {
            UpdatePuzzlePlacing();
            UpdateMove();
        }

        void OnDisable()
        {
            UpdateSystemComponent.Instance.OnUpdate -= HandleUpdate;
            _input.OnMovementInput -= HandleMoveInput;
            _input.OnRotationInput -= HandleRotateInput;
            _input.OnSoftDropInput -= HandleSoftDropInput;
            _input.OnHardDropInput -= HandleHardDrop;
            _input.OnSavePuzzleInput -= HandlePuzzleSave;
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
            var puzzle = _puzzleFeeder.SavePuzzle(_puzzleMover.ActivePuzzle.PuzzleData);
            if (puzzle != null)
                _puzzleMover.SetActivePuzzle(puzzle, _spawnPosition);
        }
    }
}
