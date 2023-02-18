using UnityEngine;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.UI
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] private LevelCompleteDialog completeDialog;

        private void Awake()
        {
            EventManager.GameStateChanged += OnGameStateChanged;
        }

        private void OnApplicationQuit()
        {
            EventManager.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState state)
        {
            if (state == GameState.Won)
            {
                completeDialog.Init();
            }
        }
    }
}
