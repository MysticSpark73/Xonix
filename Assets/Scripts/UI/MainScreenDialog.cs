using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xonix.Core;
using Xonix.Data;

namespace Xonix
{
    public class MainScreenDialog : MonoBehaviour
    {
        [SerializeField] RectTransform self;
        [SerializeField] TextMeshProUGUI insertACoin;
        [SerializeField] Button button;

        private Color transparent = new Color(0, 0, 0, 0);

        private void Awake()
        {
            button.onClick.AddListener(ResetGame);
            insertACoin.DOFade(0, 1).SetLoops(-1, LoopType.Yoyo);
        }

        public void Init()
        {
            gameObject.SetActive(true);
        }

        public void Hide() 
        {
            gameObject.SetActive(false);
        }

        private void ResetGame() 
        {
            Hide();
            Parameters.ResetScore();
            Parameters.ResetPlayerHP();
            Parameters.ResetGames();
            GameManager.Instance.ResetBoard();
            GameManager.Instance.StartLevel(Parameters.level_games_played);
        }

    }
}
