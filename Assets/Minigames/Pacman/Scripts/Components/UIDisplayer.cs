using UnityEngine;
using TMPro;

namespace gamer.pacman
{
    public class UIDisplayer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] TextMeshProUGUI _livesText;

        void Start()
        {
            _livesText.text = PacmanWorld.Instance.StartingLives.ToString();

            PointsCounter.OnPointsCollected += HandlePointsCollected;
            PacmanWorld.Instance.OnPlayerLiveChanged += HandlePlayerDeath;
        }

        void HandlePointsCollected(int points)
        {
            _scoreText.text = points.ToString();
        }

        void HandlePlayerDeath(int lives)
        {
            _livesText.text = lives.ToString();
        }
    }
}
