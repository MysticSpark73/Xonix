using UnityEngine;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Common.Grid.Grid grid;
        private void Awake()
        {
            EventManager.Swipe += OnSwipe;
            grid.CreateGrid();
            Parameters.SwitchGameState(GameState.Playing);
        }

        private void OnApplicationQuit()
        {
            EventManager.Swipe -= OnSwipe;
        }

        // Start is called before the first frame update
        void Start()
        {
            //InitIALIZE grid and prepare game
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void OnSwipe(Vector2 swipe) 
        {
            Debug.Log($"Swiped {swipe}");
        }
    }
}
