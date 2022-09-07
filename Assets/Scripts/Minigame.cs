using UnityEngine;
using UnityEngine.SceneManagement;

namespace gamer
{
    [CreateAssetMenu(fileName = "/Minigame")]
    public class Minigame : ScriptableObject
    {
        [SerializeField] string _sceneName;
        [SerializeField] RenderTexture _minigameTexture;

        public string SceneName => _sceneName;
        public RenderTexture MinigameTexture => _minigameTexture;

        public void StartMinigame()
        {
            SceneManager.LoadSceneAsync(_sceneName, LoadSceneMode.Additive);
        }

        public void StopMinigame()
        {
            SceneManager.UnloadSceneAsync(_sceneName);
        }
    }
}
