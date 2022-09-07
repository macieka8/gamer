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
        [SerializeField] PlayerInputController _input;

        public string SceneName => _sceneName;
        public RenderTexture MinigameTexture => _minigameTexture;

        public void StartMinigame()
        {
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
            _input.SetActiveActionMap(_inputActionMapName, true);
        }

        public void StopMinigame()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
            _input.RestoreDefaultActionMap();
        }
    }
}
