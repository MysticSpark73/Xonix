using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xonix.Common.Characters;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.Common.Grid
{
    public class GridBuilder : MonoBehaviour
    {
        [SerializeField] private Image palyArea;
        [SerializeField] private RectTransform tilesContainer;
        [SerializeField] private RectTransform playerContainer;
        [SerializeField] private RectTransform enemiesContainer;
        [SerializeField] private GameObject playerPrefab;

        private Player player;
        private Vector2 playerGridPos;
        private float playerMoveTime;
        private float enemiesMoveTime;

        private List<Vector2> trailTiles;
        private List<Enemy> enemies;
        private List<List<Tile>> tileMap;

        public void Init() 
        {
            tileMap = new List<List<Tile>>();
            trailTiles = new List<Vector2>();
            enemies = new List<Enemy>();
            EventManager.TookDamage += OnTookDamage;
        }

        private void Awake()
        {
            
        }

        public void BuildGrid()
        {
            int horizontalTilesCount = (int)(palyArea.rectTransform.rect.width / Parameters.grid_tile_size);
            int verticalTilesCount = (int)(palyArea.rectTransform.rect.height / Parameters.grid_tile_size);


            for (int i = 0; i < verticalTilesCount; i++)
            {
                List<Tile> row = new List<Tile>();
                for (int j = 0; j < horizontalTilesCount; j++)
                {
                    row.Add(CreateTile(j, i));
                }
                tileMap.Add(row);
            }
            playerGridPos = FindSpawnPos();
            SetupGrid();
            trailTiles.Add(FindSpawnPos());
            Parameters.SetFillAmount(GetFilledAmount());
        }

        public void ClearGrid()
        {
            for (int i = 0; i < tileMap.Count; i++)
            {
                for (int j = 0; j < tileMap[i].Count; j++)
                {
                    tileMap[i][j].SetColor(Parameters.grid_color_water);
                    //also kill enemies and respawn player here
                }
            }
        }

        private void SetupGrid()
        {
            //possible different configurations of setup ground on grid (manual/ pixel map)
            for (int i = 0; i < tileMap.Count; i++)
            {
                for (int j = 0; j < tileMap[i].Count; j++)
                {
                    if (i <= 1 || i >= tileMap.Count - 2 || j <= 1 || j >= tileMap[i].Count - 2)
                    {
                        tileMap[i][j].SetColor(Parameters.grid_color_ground);
                    }
                }
            }
        }

        private float GetFilledAmount() 
        {
            int counter = 0;
            foreach (var row in tileMap)
            {
                counter += row.Count(t => t.GetColor() == Parameters.grid_color_ground);
            }
            return counter / (tileMap.Count * tileMap[0].Count);
        }

        public Vector2 FindSpawnPos() => Vector2.right * tileMap[0].Count / 2;

        public Vector2 FindEnemySpawnPos(EnemyType enemyType) 
        {
            List<Vector2> groundTiles = new List<Vector2>();
            List<Vector2> waterTiles = new List<Vector2>();

            for (int i = 0; i < tileMap.Count; i++)
            {
                for (int j = 0; j < tileMap[i].Count; j++)
                {
                    if (IsExcluded(new Vector2(j, i)))
                    {
                        continue;
                    }
                    if (tileMap[i][j].GetColor() == Parameters.grid_color_water)
                    {
                        waterTiles.Add(new Vector2(j, i));
                    }
                    if (tileMap[i][j].GetColor() == Parameters.grid_color_ground)
                    {
                        groundTiles.Add(new Vector2(j, i));
                    }
                }
            }

            switch (enemyType)
            {
                case EnemyType.Water:
                    return waterTiles[Random.Range(0, waterTiles.Count - 1)];
                case EnemyType.Ground:
                    return groundTiles[Random.Range(0, groundTiles.Count - 1)];
                default:
                    return Vector2.one * -1;
            }
        }

        private bool IsExcluded(Vector2 exc) 
        {
            if (exc == FindSpawnPos())
            {
                return true;
            }
            foreach (var enemy in enemies)
            {
                if (enemy.GridPos == exc)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetPlayer(Player player) => this.player = player;

        public void AddEnemy(Enemy enemy)
        { 
            enemies.Add(enemy);
            enemy.SetPos(FindEnemySpawnPos(enemy.EnemyType));
        }

        public Color GetTileColor(int x, int y)
        {
            if (x < 0 || x >= tileMap[0].Count || y < 0 || y >= tileMap.Count)
            {
                return Color.magenta;
            }
            return tileMap[y][x].GetColor();
        }

        private Tile CreateTile(int x, int y)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(tilesContainer);
            go.name = $"Tile [{x}][{y}]";
            Tile tile = go.AddComponent<Tile>();
            tile.Init();
            tile.SetSize(Parameters.grid_tile_size);
            tile.SetColor(Parameters.grid_color_water);
            tile.setGridPos(x, y);
            return tile;
        }

        private void Update()
        {
            MovePlayer();
            MoveEnemies();
        }

        private void MovePlayer()
        {
            if (player.MoveDirection == Vector2.zero)
            {
                playerMoveTime = 0;
                return;
            }
            playerMoveTime += Time.deltaTime;
            if (playerMoveTime >= Parameters.player_move_delay)
            {
                playerMoveTime = 0;
                if (IsCanMove(playerGridPos, player.MoveDirection))
                {
                    playerGridPos += player.MoveDirection * new Vector2(1, -1);
                    trailTiles.Add(playerGridPos);
                    player.SetPos(playerGridPos);
                    CheckGroundColor(playerGridPos);
                }
                else
                {
                    player.Stop();
                    ClearTrail();
                }
            }

        }

        private void MoveEnemies() 
        {
            enemiesMoveTime += Time.deltaTime;
            if (enemiesMoveTime >= Parameters.enemies_move_delay)
            {
                enemiesMoveTime = 0;
            }
            else
            {
                return;
            }
            foreach (var enemy in enemies)
            {
                enemy.Bounce(FindFlipVector(FindCornerTiles(enemy.GridPos, enemy.MoveDirection, enemy.EnemyType)));
                enemy.SetPos(enemy.GridPos + enemy.MoveDirection * new Vector2(1, -1));
            }
        }

        private bool[] FindCornerTiles(Vector2 pos, Vector2 dir, EnemyType enemyType) 
        {
            //x, -y, x-y
            bool[] cornerTiles = new bool[3];
            Color tempCol;
            if (pos.x + dir.x <0 || pos.x + dir.x >= tileMap[0].Count)
            {
                cornerTiles[0] = true;
                cornerTiles[2] = true;
            }
            else
            {
                tempCol = tileMap[ (int) pos.y][ (int) (pos.x + dir.x)].GetColor();
                if (tempCol == Parameters.grid_color_ground)
                {
                    if (enemyType == EnemyType.Water)
                    {
                        cornerTiles[0] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_water)
                {
                    if (enemyType == EnemyType.Ground)
                    {
                        cornerTiles[0] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_trail)
                {
                    player.TakeDamage();
                    cornerTiles[0] = true;
                }
            }
            if (pos.y - dir.y < 0 || pos.y - dir.y >= tileMap.Count)
            {
                cornerTiles[1] = true;
                cornerTiles[2] = true;
            }
            else
            {
                tempCol = tileMap[ (int) (pos.y - dir.y)][ (int) pos.x].GetColor();
                if (tempCol == Parameters.grid_color_ground)
                {
                    if (enemyType == EnemyType.Water)
                    {
                        cornerTiles[1] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_water)
                {
                    if (enemyType == EnemyType.Ground)
                    {
                        cornerTiles[1] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_trail)
                {
                    cornerTiles[1] = true;
                    player.TakeDamage();
                }
            }
            if (!cornerTiles[2])
            {
                tempCol = tileMap[(int)(pos.y - dir.y)][(int)(pos.x + dir.x)].GetColor();
                if (tempCol == Parameters.grid_color_ground)
                {
                    if (enemyType == EnemyType.Water)
                    {
                        cornerTiles[2] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_water)
                {
                    if (enemyType == EnemyType.Ground)
                    {
                        cornerTiles[2] = true;
                    }
                }
                if (tempCol == Parameters.grid_color_trail)
                {
                    cornerTiles[2] = true;
                    player.TakeDamage();
                }
            }
            return cornerTiles;
        }

        private Vector2 FindFlipVector(bool[] cornerTiles) 
        {
            if (cornerTiles.All(ct => ct == true))
            {
                return Vector2.one * -1;
            }
            if (cornerTiles[2])
            {
                if (cornerTiles[0])
                {
                    return new Vector2(-1, 1);
                }
                if (cornerTiles[1])
                {
                    return new Vector2(1, -1);
                }
                return Vector2.one * -1;
            }
            return Vector2.one;
        }

        private Color GetNextTilecolor(Vector2 pos, Vector2 dir) 
        {
            Vector2 res =  pos + dir * new Vector2(1, -1);
            if (res.x < 0 || res.y < 0 || res.x >= tileMap[0].Count || res.y >= tileMap.Count)
            {
                return Color.magenta;
            }
            return tileMap[ (int) res.y][(int) res.x].GetColor();
        }

        private bool IsCanMove(Vector2 gridPos, Vector2 direction) 
        {
            Vector2 temp = gridPos + direction * new Vector2(1, -1);
            if (temp.x < 0 || temp.y < 0 || temp.x >= tileMap[0].Count || temp.y >= tileMap.Count)
            {
                return false;
            }
            return true;
        }

        private void CheckGroundColor(Vector2 playerGridPos)
        {
            Color color = GetTileColor((int)playerGridPos.x, (int)playerGridPos.y);

            if (color == Parameters.grid_color_water)
            {
                tileMap[(int)playerGridPos.y][(int)playerGridPos.x].SetColor(Parameters.grid_color_trail);
                if (!player.GetIsSwiming())
                {
                    player.SetIsSwiming(true);
                }
            }

            if (color == Parameters.grid_color_ground)
            {
                if (player.GetIsSwiming())
                {
                    player.Stop();
                    ClearTrail();
                }
            }

            if (color == Parameters.grid_color_trail)
            {
                player.TakeDamage();
            }
        }

        private void ClearTrail(bool delete = false) 
        {
            foreach (var vec in trailTiles)
            {
                if (tileMap[(int)vec.y][(int)vec.x].GetColor() != Parameters.grid_color_ground)
                {
                    tileMap[(int)vec.y][(int)vec.x].SetColor(delete ? Parameters.grid_color_water : Parameters.grid_color_ground);
                }
            }
            trailTiles.Clear();
            trailTiles.Add(playerGridPos);
        }

        private void OnTookDamage() 
        {
            playerGridPos = FindSpawnPos();
            ClearTrail(true);
            player.SetPos(playerGridPos);
        }

    }
}
