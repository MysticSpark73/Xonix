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

        public int GridWidth => tileMap[0].Count;
        public int GridHeight => tileMap.Count;

        private GridCharactersMover charactersMover;
        private List<Vector2> trailTiles;
        private List<List<Tile>> tileMap;

        public void Init() 
        {
            tileMap = new List<List<Tile>>();
            trailTiles = new List<Vector2>();
            charactersMover = new GridCharactersMover(this);
            EventManager.PlayerTakenDamage += OnTakenDamage;
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
            charactersMover.SetPlayerPosition(FindSpawnPos());
            SetupGrid();
            trailTiles.Add(charactersMover.GetPlayerSpawnPos());
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

        public void AddToTrail(Vector2 pos) 
        {
            trailTiles.Add(pos);
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
                    if (charactersMover.IsEnemySpawnExcluded(new Vector2(j, i)))
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

        public void SetPlayer(Player player) => charactersMover.SetPlayer(player);

        public void AddEnemy(Enemy enemy) => charactersMover.AddEnemy(enemy);

        public Color GetTileColor(Vector2 index)
        {
            if (index.x < 0 || index.x >= GridWidth || index.y < 0 || index.y >= GridHeight)
            {
                return Color.magenta;
            }
            return tileMap[(int)index.y][(int)index.x].GetColor();
        }

        public void SetTileColor(Vector2 index, Color color) 
        {
            if (index.x < 0 || index.x >= GridWidth || index.y < 0 || index.y >= GridHeight)
            {
                return;
            }
            tileMap[(int)index.y][(int)index.x].SetColor(color);
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
            charactersMover.MovePlayer();
            charactersMover.MoveEnemies();
        }

        public bool[] FindCornerTiles(Vector2 pos, Vector2 dir, EnemyType enemyType) 
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
                cornerTiles[0] = IsObstacleColor(tempCol, enemyType);
                if (new Vector2(pos.x + dir.x, pos.y) == charactersMover.GetPlayerPos())
                {
                    charactersMover.Damage();
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
                cornerTiles[1] = IsObstacleColor(tempCol, enemyType);
                if (new Vector2(pos.x ,pos.y - dir.y) == charactersMover.GetPlayerPos())
                {
                    cornerTiles[1] = true;
                    charactersMover.Damage();
                }
            }
            if (!cornerTiles[2])
            {
                tempCol = tileMap[(int)(pos.y - dir.y)][(int)(pos.x + dir.x)].GetColor();
                cornerTiles[2] = IsObstacleColor(tempCol, enemyType);
                if (new Vector2(pos.x + dir.x, pos.y - dir.y) == charactersMover.GetPlayerPos())
                {
                    cornerTiles[2] = true;
                    charactersMover.Damage();
                }
            }
            return cornerTiles;
        }

        public bool IsObstacleColor(Color color, EnemyType enemy) 
        {
            if (color == Parameters.grid_color_ground)
            {
                if (enemy == EnemyType.Water)
                {
                    return true;
                }
            }
            if (color == Parameters.grid_color_water)
            {
                if (enemy == EnemyType.Ground)
                {
                    return true;
                }
            }
            if (color == Parameters.grid_color_trail)
            {
                charactersMover.Damage();
                return true;
            }
            return false;
        }

        public void ClearTrail(Vector2 currentPlayerPos, bool delete = false) 
        {
            foreach (var vec in trailTiles)
            {
                if (tileMap[(int)vec.y][(int)vec.x].GetColor() != Parameters.grid_color_ground)
                {
                    tileMap[(int)vec.y][(int)vec.x].SetColor(delete ? Parameters.grid_color_water : Parameters.grid_color_ground);
                }
            }
            trailTiles.Clear();
            trailTiles.Add(currentPlayerPos);
        }

        private void OnTakenDamage() 
        {
            charactersMover.OnTakenDamage();
        }

    }
}
