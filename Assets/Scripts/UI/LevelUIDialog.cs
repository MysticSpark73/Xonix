using TMPro;
using UnityEngine;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.UI
{
    public class LevelUIDialog : MonoBehaviour
    {
        [SerializeField] private RectTransform self;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI healthText;
        [SerializeField] private TextMeshProUGUI fillText;
        [SerializeField] private TextMeshProUGUI timeText;

        private void Awake()
        {
            EventManager.ScoreChanged += OnScoreChanged;
            EventManager.PlayerTakenDamage += OnPlayerTakenDamage;
            EventManager.FillChanged += OnFillChanged;
            EventManager.TimeChanged += OnTimeChanged;
        }

        private void OnApplicationQuit()
        {
            EventManager.ScoreChanged -= OnScoreChanged;
            EventManager.PlayerTakenDamage -= OnPlayerTakenDamage;
            EventManager.FillChanged -= OnFillChanged;
            EventManager.TimeChanged -= OnTimeChanged;
        }

        private void OnScoreChanged() => scoreText.text = $"{Parameters.level_dialog_score_text}{Parameters.level_score}";

        private void OnPlayerTakenDamage() => healthText.text = $"{Parameters.level_dialog_health_text}{Parameters.player_hp}";

        private void OnFillChanged() => fillText.text = $"{Parameters.level_dialog_fill_text}{(int) (Parameters.current_fill_amount * 100)}%";

        private void OnTimeChanged() => timeText.text = $"{Parameters.level_dialog_time_text}{Parameters.level_time_seconds}";

    }
}
