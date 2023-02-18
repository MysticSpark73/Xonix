using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xonix.Core;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.UI
{
    public class LevelCompleteDialog : MonoBehaviour
    {
        [SerializeField] private RectTransform self;
        [SerializeField] private Button btn;
        [SerializeField] private TextMeshProUGUI touchToContinueText;

        private void Awake()
        {
            btn.onClick.AddListener(StartLevel);
            touchToContinueText.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        }

        public void Init() 
        {
            gameObject.SetActive(true);
        }

        private void StartLevel() 
        {
            GameManager.Instance.ResetBoard();
            GameManager.Instance.StartLevel(Parameters.level_games_played);
            gameObject.SetActive(false);
        }

        
    }
}
