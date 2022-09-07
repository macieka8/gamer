using UnityEngine;
using UnityEngine.EventSystems;

namespace gamer
{
    public class SelectOnEnable : MonoBehaviour
    {
        [SerializeField] GameObject _gameObjectToSelect;

        void OnEnable()
        {
            EventSystem.current.SetSelectedGameObject(_gameObjectToSelect);
        }
    }
}
