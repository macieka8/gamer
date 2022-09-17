using UnityEngine;

namespace gamer.tetris
{
    public class UpdateSystemComponent : MonoBehaviour
    {
        static UpdateSystemComponent _instance;
        public static event System.Action OnUpdate
        {
            add { _instance._updateSystem.OnUpdate += value; }
            remove { _instance._updateSystem.OnUpdate -= value; }
        }

        [SerializeField] float _timeBetweenUpdatesInSeconds;
        UpdateSystem _updateSystem;
        float _timeLeftToUpdate;

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _updateSystem = new UpdateSystem();
                _timeLeftToUpdate = _timeBetweenUpdatesInSeconds;
            }
        }

        void Update()
        {
            if (_timeLeftToUpdate <= 0f)
            {
                _timeLeftToUpdate = _timeBetweenUpdatesInSeconds;
                _updateSystem.Update();
            }
            _timeLeftToUpdate -= Time.deltaTime;
        }
    }
}
