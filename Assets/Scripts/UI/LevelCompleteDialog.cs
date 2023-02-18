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

        private void Awake()
        {
            btn.onClick.AddListener(StartLevel);
        }

        public void Init() 
        {
            gameObject.SetActive(true);
        }

        private void StartLevel() 
        {
            Debug.Log("Pressed Button!");
            GameManager.Instance.ResetBoard();
            GameManager.Instance.StartLevel(Parameters.level_games_played);
            gameObject.SetActive(false);
        }

        
    }
}
