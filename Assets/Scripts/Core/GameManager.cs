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

        public static GameManager Instance;

        private Player player;
        private EnemyPool pool;
        private float timePassed = 0;

        private void Awake()
        {
            Instance = this;
            grid.Init();
            pool = new EnemyPool(this);

            CreatePlayer();
            grid.SetPlayer(player);
            grid.BuildGrid();
            player.Init(grid.FindSpawnPos());
            FillPool(50);
            StartLevel(Parameters.level_games_played);
        }

        public void StartLevel(int levelIndex) 
        {
            if (levelIndex <=1)
            {
                SpawnEnemies(EnemyType.Water, levelIndex + 1);
            }
            else
            {
                SpawnEnemies(EnemyType.Ground, (levelIndex + 1) / 3);
                SpawnEnemies(EnemyType.Water, levelIndex + 1 - ((levelIndex + 1) / 3));
            }
            Parameters.ResetTime();
            Parameters.SwitchGameState(GameState.Playing);
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

        public void ReturnToPool(Enemy enemy)
        {
            
            pool.AddToPool(enemy);
        }

        public void ResetBoard() 
        {
            grid.ClearGrid();
            grid.SetupGrid();
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

        private void Update()
        {
            if (Parameters.GameState == GameState.Playing)
            {
                timePassed += Time.deltaTime;
                if (timePassed >= 1)
                {
                    Parameters.SubtractSeconds(1);
                    timePassed -= 1;
                }
            }
        }

    }
}
