using UnityEngine;

namespace gamer
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] Minigame _minigame;
        [SerializeField] GameObject _game;
        [SerializeField] GameObject _menu;

        public void StartGame()
        {
            _game.SetActive(true);
            _menu.SetActive(false);
        }

        public void StartMenu()
        {
            _menu.SetActive(true);
            _game.SetActive(false);
        }

        public void StopMinigame()
        {
            _minigame.StopMinigame();
        }
    }
}
