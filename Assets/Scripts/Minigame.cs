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

        public string SceneName => _sceneName;
        public RenderTexture MinigameTexture => _minigameTexture;
        public Material MinigameMaterial => _minigameMaterial;
        public string InputActionMapName => _inputActionMapName;

        public event System.Action onMinigameStopped;

        public void StartMinigame()
        {
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        }

        public void StopMinigame()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
            if (_input.ActiveActionMap.name == _inputActionMapName)
                _input.RestoreDefaultActionMap();

            onMinigameStopped?.Invoke();
        }
    }
}
