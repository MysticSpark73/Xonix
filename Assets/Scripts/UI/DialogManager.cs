using UnityEngine;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.UI
{
    public class DialogManager : MonoBehaviour
    {
        [SerializeField] private LevelCompleteDialog completeDialog;
        [SerializeField] private MainScreenDialog mainScreen;

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
            switch (state)
            {
                case GameState.Playing:
                    break;
                case GameState.Won:
                    completeDialog.Init();
                    break;
                case GameState.Lost:
                    mainScreen.Init();
                    break;
            }
           
        }


    }
}
