using UnityEngine;
using Xonix.Common.Characters;
using Xonix.Data;

namespace Xonix.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Common.Grid.GridBuilder grid;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private RectTransform playerContainer;
        [SerializeField] private RectTransform enemiesContainer;

        private Player player;

        private void Awake()
        {
            CreatePlayer();
            grid.SetPlayer(player);
            grid.BuildGrid();
            player.Init(grid.FindSpawnPos());
            //Create Player
            //Setup Player
            //Create pool of enemies
            //remove later
            Parameters.SwitchGameState(GameState.Playing);
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

        private void StartLevel(int levelIndex) 
        {
            //calculate enemies amounts
            //clear grid
            //reset player
            //place enemies
            //run level
        }

        private void CreatePlayer() 
        {
            GameObject go = Instantiate(playerPrefab, playerContainer);
            Player player = go.GetComponent<Player>();
            this.player = player;
        }

        
    }
}
