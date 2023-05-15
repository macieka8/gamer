using UnityEngine;
using TMPro;

namespace gamer.pong
{
    public class ScoreUI : MonoBehaviour
    {
        [SerializeField] GoalComponent _goal;
        [SerializeField] TextMeshProUGUI _scoreText;


        void OnEnable()
        {
            HandleGoalScored(0);
        }

        void Start()
        {
            _goal.OnGoalScored += HandleGoalScored;
        }

        void OnDestroy()
        {
            _goal.OnGoalScored -= HandleGoalScored;
        }

        void HandleGoalScored(int score)
        {
            _scoreText.text = score.ToString();
        }
    }
}
