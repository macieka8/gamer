using UnityEngine;

namespace gamer.pong
{
    public class GameController : MonoBehaviour
    {
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
    }
}
