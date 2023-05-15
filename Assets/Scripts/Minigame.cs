using UnityEngine;
using UnityEngine.SceneManagement;

namespace gamer
{
    [CreateAssetMenu(fileName = "/Minigame")]
    public class Minigame : ScriptableObject
    {
        [SerializeField] string _sceneName;
        [SerializeField] string _inputActionMapName;
        [SerializeField] RenderTexture _minigameTexture;
        [SerializeField] Material _minigameMaterial;
        [SerializeField] PlayerInputController _input;
        [SerializeField] int _maxPlayerCount;

        IInputSenderMapManager _inputSenderMapManager;

        public string SceneName => _sceneName;
        public RenderTexture MinigameTexture => _minigameTexture;
        public Material MinigameMaterial => _minigameMaterial;
        public string InputActionMapName => _inputActionMapName;
        public int MaxPlayerCount => _maxPlayerCount;

        public IInputSenderMapManager InputSenderMapManager => _inputSenderMapManager;

        public event System.Action OnMinigameStopped;

        public void StartMinigame()
        {
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        }

        public void StopMinigame()
        {
            OnMinigameStopped?.Invoke();
            SceneManager.UnloadSceneAsync(_sceneName);
            if (_input.ActiveActionMap.name == _inputActionMapName)
                _input.RestoreDefaultActionMap();
        }

        public void RegisterSenderMap(IInputSenderMapManager senderMap)
        {
            _inputSenderMapManager = senderMap;
        }

        public void UnregisterSenderMap(IInputSenderMapManager senderMap)
        {
            if (_inputSenderMapManager == senderMap)
            {
                _inputSenderMapManager = null;
            }
        }
    }
}
