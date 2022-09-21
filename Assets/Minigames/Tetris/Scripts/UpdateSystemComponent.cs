using UnityEngine;
using UnityEngine.InputSystem;

namespace gamer.tetris
{
    public enum DropState
    {
        Normal,
        SoftDrop,
    }

    public class UpdateSystemComponent : MonoBehaviour
    {
        static UpdateSystemComponent _instance;
        public static event System.Action OnUpdate
        {
            add { _instance._updateSystem.OnUpdate += value; }
            remove { _instance._updateSystem.OnUpdate -= value; }
        }

        [SerializeField] PuzzleMoverComponent _puzzleMover;
        [SerializeField] InputActionReference _softDropInputAction;
        [SerializeField] InputActionReference _hardDropInputAction;

        [SerializeField] float _normalFallTime;
        [SerializeField] float _softDropFallTime;

        UpdateSystem _updateSystem;
        DropState _dropState;
        float _timeLeftToUpdate;
        float _currentUpdateTime;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _updateSystem = new UpdateSystem();
                _currentUpdateTime = _normalFallTime;
                _timeLeftToUpdate = _currentUpdateTime;
            }
        }
        void OnEnable()
        {
            //todo: remove enable
            _softDropInputAction.action.started += HandleSoftDropStarted;
            _softDropInputAction.action.canceled += HandleSoftDropCanceled;
            _softDropInputAction.action.Enable();

            _hardDropInputAction.action.performed += HandleHardDrop;
            _hardDropInputAction.action.Enable();
        }
        void OnDisable()
        {
            _softDropInputAction.action.started -= HandleSoftDropStarted;
            _softDropInputAction.action.canceled -= HandleSoftDropStarted;
            _hardDropInputAction.action.performed -= HandleHardDrop;
        }

        void Update()
        {
            if (_timeLeftToUpdate <= 0f)
            {
                _timeLeftToUpdate = _currentUpdateTime;
                _updateSystem.Update();
            }
            _timeLeftToUpdate -= Time.deltaTime;
        }

        void HandleSoftDropStarted(InputAction.CallbackContext ctx)
        {
            if (_dropState != DropState.Normal) return;

            _dropState = DropState.SoftDrop;
            _currentUpdateTime = _softDropFallTime;
            _timeLeftToUpdate = _currentUpdateTime;
            _updateSystem.Update();
        }

        void HandleSoftDropCanceled(InputAction.CallbackContext ctx)
        {
            if (_dropState != DropState.SoftDrop) return;

            _dropState = DropState.Normal;
            _currentUpdateTime = _normalFallTime;
            _timeLeftToUpdate = _currentUpdateTime;
        }

        void HandleHardDrop(InputAction.CallbackContext ctx)
        {
            _puzzleMover.HardDropDown();
            _updateSystem.Update();
        }
    }
}
