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
            charactersMover.SetPlayerSpawnPos(FindSpawnPos());
            SetupGrid();
            trailTiles.Add(charactersMover.GetPlayerSpawnPos());
            //float tempFill = Parameters.current_fill_amount;
            //Parameters.SetFillAmount(GetFilledAmount());
            //Parameters.AddScore((int) ((Parameters.current_fill_amount - tempFill) * 100));
        }

        public void ClearGrid()
        {
            for (int i = 0; i < tileMap.Count; i++)
            {
                for (int j = 0; j < tileMap[i].Count; j++)
                {
                    tileMap[i][j].SetColor(Parameters.grid_color_water);
                    charactersMover.RemoveEnemies();
                    charactersMover.ResetPlayer();
                }
            }
        }

        public void SetupGrid()
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

        public void AddToTrail(Vector2 pos)
        {
            trailTiles.Add(pos);
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

        public bool[] FindCornerTiles(Vector2 pos, Vector2 dir, EnemyType enemyType)
        {
            //x, -y, x-y
            bool[] cornerTiles = new bool[3];
            Color tempCol;
            if (pos.x + dir.x < 0 || pos.x + dir.x >= tileMap[0].Count)
            {
                cornerTiles[0] = true;
                cornerTiles[2] = true;
            }
            else
            {
                tempCol = tileMap[(int)pos.y][(int)(pos.x + dir.x)].GetColor();
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
                tempCol = tileMap[(int)(pos.y - dir.y)][(int)pos.x].GetColor();
                cornerTiles[1] = IsObstacleColor(tempCol, enemyType);
                if (new Vector2(pos.x, pos.y - dir.y) == charactersMover.GetPlayerPos())
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
            //repaint
            foreach (var vec in trailTiles)
            {
                if (tileMap[(int)vec.y][(int)vec.x].GetColor() != Parameters.grid_color_ground)
                {
                    tileMap[(int)vec.y][(int)vec.x].SetColor(delete ? Parameters.grid_color_water : Parameters.grid_color_ground);
                }
            }
            if (!delete)
            {
                List<Vector2> points = FindTrailFillPoint(trailTiles);
                if (points.Count > 0)
                {
                    int[] areas = new int[points.Count];
                    for (int i = 0; i < points.Count; i++)
                    {
                        areas[i] = CalculateArea(points[i]);
                    }
                    FillArea(points[areas[0] < areas[1] ? 0 : 1]);
                }
                float tempFill = Parameters.current_fill_amount;
                Parameters.SetFillAmount(GetFilledAmount());
                Parameters.AddScore((int)((Parameters.current_fill_amount - tempFill) * 100));
            }
            trailTiles.Clear();
            trailTiles.Add(currentPlayerPos);
        }

        private List<Vector2> FindTrailFillPoint(List<Vector2> trail)
        {
            List<Vector2> points = new List<Vector2>();
            foreach (var t in trail)
            {
                if (t.x + 1 < GridWidth && t.x - 1 > 0)
                {
                    if (GetTileColor(new Vector2(t.x + 1, t.y)) == GetTileColor(new Vector2(t.x - 1, t.y))
                        && GetTileColor(new Vector2(t.x + 1, t.y)) == Parameters.grid_color_water)
                    {
                        points.Add(new Vector2(t.x + 1, t.y));
                        points.Add(new Vector2(t.x - 1, t.y));
                        return points;
                    }
                }
                if (t.y + 1 < GridHeight && t.y - 1 > 0)
                {
                    if (GetTileColor(new Vector2(t.x, t.y + 1)) == GetTileColor(new Vector2(t.x, t.y - 1))
                        && GetTileColor(new Vector2(t.x, t.y + 1)) == Parameters.grid_color_water)
                    {
                        points.Add(new Vector2(t.x, t.y + 1));
                        points.Add(new Vector2(t.x, t.y - 1));
                        return points;
                    }
                }

            }
            return points;
        }

        private int CalculateArea(Vector2 point, List<Vector2> excl = null)
        {
            int area = 0;
            if (point.x >= GridWidth || point.x < 0 || point.y >= GridHeight || point.y < 0)
            {
                return area;
            }
            if (GetTileColor(point) != Parameters.grid_color_water)
            {
                return area;
            }
            if (excl == null)
            {
                excl = new List<Vector2>();
            }
            area++;
            excl.Add(point);

            Vector2 xP = new Vector2(point.x + 1, point.y);
            Vector2 xM = new Vector2(point.x - 1, point.y);
            Vector2 yP = new Vector2(point.x, point.y + 1);
            Vector2 yM = new Vector2(point.x, point.y - 1);
            if (!excl.Contains(xP))
            {
                area += CalculateArea(xP, excl);
            }
            if (!excl.Contains(xM))
            {
                area += CalculateArea(xM, excl);
            }
            if (!excl.Contains(yP))
            {
                area += CalculateArea(yP, excl);
            }
            if (!excl.Contains(yM))
            {
                area += CalculateArea(yM, excl);
            }
            return area;
        }

        private void FillArea(Vector2 point)
        {
            if (point.x >= GridWidth || point.x < 0 || point.y >= GridHeight || point.y < 0)
            {
                return;
            }
            if (GetTileColor(point) == Parameters.grid_color_water)
            {
                SetTileColor(point, Parameters.grid_color_ground);
                charactersMover.CheckKillEnemy(point);
            }
            else
            {
                return;
            }
            FillArea(new Vector2(point.x + 1, point.y));
            FillArea(new Vector2(point.x - 1, point.y));
            FillArea(new Vector2(point.x, point.y + 1));
            FillArea(new Vector2(point.x, point.y - 1));

        }

        private float GetFilledAmount() 
        {
            int counter = 0;
            foreach (var row in tileMap)
            {
                counter += row.Count(t => t.GetColor() == Parameters.grid_color_ground);
            }
            return (float)((float)counter /(float)(tileMap.Count * tileMap[0].Count));
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
            if (Parameters.GameState == GameState.Playing)
            {
                charactersMover.MovePlayer();
                charactersMover.MoveEnemies();
            }
        }

        private void OnTakenDamage() 
        {
            charactersMover.OnTakenDamage();
        }

        private void OnApplicationQuit()
        {
            EventManager.PlayerTakenDamage -= OnTakenDamage;
        }

    }
}
