using UnityEngine;
using UnityEngine.EventSystems;

namespace gamer
{
    public class SelectNavigationUI : MonoBehaviour
    {
        [SerializeField] GameObject _defaultSelectedGameObject;

        EventSystem _eventSystem;
        GameObject _selectedGameObject;

        void Start()
        {
            _eventSystem = EventSystem.current;
        }

        void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_defaultSelectedGameObject);
        }

        void Update()
        {
            if (_eventSystem.currentSelectedGameObject != null && _eventSystem.currentSelectedGameObject != _selectedGameObject)
                _selectedGameObject = _eventSystem.currentSelectedGameObject;
            else if (_selectedGameObject != null && _eventSystem.currentSelectedGameObject == null)
                _eventSystem.SetSelectedGameObject(_selectedGameObject);
        }
    }
}
