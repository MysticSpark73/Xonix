using UnityEngine;
using Xonix.Common.Characters;
using Xonix.Data;

namespace Xonix.Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private Common.Grid.GridBuilder grid;
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private RectTransform playerContainer;
        [SerializeField] private RectTransform enemiesContainer;
        [SerializeField] private RectTransform enemiesPoolContainer;

        private Player player;
        private EnemyPool pool;

        private void Awake()
        {
            grid.Init();
            pool = new EnemyPool(this);

            CreatePlayer();
            grid.SetPlayer(player);
            grid.BuildGrid();
            player.Init(grid.FindSpawnPos());
            FillPool(50);
            StartLevel(Parameters.level_games_played);
            //remove later
            Parameters.SwitchGameState(GameState.Playing);
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        private void StartLevel(int levelIndex) 
        {

            if (levelIndex >1)
            {
                //spawn 2/3 water and 1/3 ground
            }
            //1 or 2 water
            SpawnEnemies(EnemyType.Water, levelIndex + 3);
            SpawnEnemies(EnemyType.Ground, 2);
        }

        private void CreatePlayer() 
        {
            GameObject go = Instantiate(playerPrefab, playerContainer);
            Player player = go.GetComponent<Player>();
            this.player = player;
        }

        private void FillPool(int count) 
        {
            for (int i = 0; i < count; i++)
            {
                GameObject go = Instantiate(enemyPrefab, enemiesPoolContainer);
                Enemy en = go.GetComponent<Enemy>();
                go.SetActive(false);
                pool.AddToPool(en);
            }
        }

        private void SpawnEnemies(EnemyType enemy, int count) 
        {
            for (int i = 0; i < count; i++)
            {
                Enemy en = pool.SpawnFromPool();
                en.Init(enemy, enemiesContainer);
                grid.AddEnemy(en);
            }
        }

        
    }
}
