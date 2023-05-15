using UnityEngine;
using UnityEngine.EventSystems;

namespace gamer
{
    public class SelectNavigationUI : MonoBehaviour
    {
        [SerializeField] PlayerInputController _playerInputController;
        [SerializeField] InputActionMapReference _owner;
        [SerializeField] GameObject _defaultSelectedGameObject;

        EventSystem _eventSystem;
        GameObject _selectedGameObject;

        void Awake()
        {
            _eventSystem = EventSystem.current;
        }

        void OnEnable()
        {
            _playerInputController.OnActionMapChanged += HandleActionMapChanged;

            if (_playerInputController.ActiveActionMap == _owner.Value)
            {
                EventSystem.current.SetSelectedGameObject(_defaultSelectedGameObject);
                _selectedGameObject = EventSystem.current.currentSelectedGameObject;
            }
            else
            {
                _selectedGameObject = _defaultSelectedGameObject;
            }
        }

        void OnDisable()
        {
            _playerInputController.OnActionMapChanged -= HandleActionMapChanged;
        }

        void Update()
        {
            if (_playerInputController.ActiveActionMap != _owner.Value) return;
            if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject != _selectedGameObject)
                _selectedGameObject = _eventSystem.currentSelectedGameObject;
            else if (_selectedGameObject != null && _eventSystem.currentSelectedGameObject == null)
                _eventSystem.SetSelectedGameObject(_selectedGameObject);
        }

        void HandleActionMapChanged()
        {
            if (_playerInputController.ActiveActionMap != _owner.Value) return;
            if (_selectedGameObject != null)
                _eventSystem.SetSelectedGameObject(_selectedGameObject);
        }
    }
}
