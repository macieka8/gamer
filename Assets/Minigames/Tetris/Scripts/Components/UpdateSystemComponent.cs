using UnityEngine;

namespace gamer.tetris
{
    public class UpdateSystemComponent : MonoBehaviour
    {
        static UpdateSystemComponent _instance;
        public static UpdateSystemComponent Instance => _instance;
        [SerializeField] float _updateTime;

        UpdateSystem _updateSystem;
        float _timeLeftToUpdate;
        float _currentUpdateTime;

        public event System.Action OnUpdate
        {
            add { _instance._updateSystem.OnUpdate += value; }
            remove { _instance._updateSystem.OnUpdate -= value; }
        }

        void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
                _updateSystem = new UpdateSystem();
                _currentUpdateTime = _updateTime;
            }
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

        public void SetUpdateTime(float time)
        {
            _currentUpdateTime = time;
            _timeLeftToUpdate = _currentUpdateTime;
            _updateSystem.Update();
        }

        public void SetDefaultUpdateTime()
        {
            _currentUpdateTime = _updateTime;
            _timeLeftToUpdate = _currentUpdateTime;
            _updateSystem.Update();
        }

        public void ForceUpdate()
        {
            _instance._updateSystem.Update();
        }
    }
}
