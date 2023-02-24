using UnityEngine;
using TMPro;

namespace gamer.pacman
{
    public class UIDisplayer : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _scoreText;

        void Start()
        {
            PointsCounter.OnPointsCollected += HandlePointsCollected;
        }

        void HandlePointsCollected(int points)
        {
            _scoreText.text = points.ToString();
        }
    }
}
