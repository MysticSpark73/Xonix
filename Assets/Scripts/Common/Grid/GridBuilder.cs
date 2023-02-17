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

        private GridPlayerController playerController;
        private Player player;
        private Vector2 playerGridPos;
        private float playerMoveTime;

        private List<Vector2> trailTiles;

        private List<List<Tile>> tileMap;

        private void Awake()
        {
            playerController = new GridPlayerController(this);
            trailTiles = new List<Vector2>();
            trailTiles.Add(FindSpawnPos());
            EventManager.TookDamage += OnTookDamage;
        }

        public void BuildGrid()
        {
            int horizontalTilesCount = (int)(palyArea.rectTransform.rect.width / Parameters.grid_tile_size);
            int verticalTilesCount = (int)(palyArea.rectTransform.rect.height / Parameters.grid_tile_size);

            tileMap = new List<List<Tile>>();

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

        public void SetPlayer(Player player) => this.player = player;

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
        }

        private void MovePlayer()
        {
            if (player.GetMoveDirection() == Vector2.zero)
            {
                playerMoveTime = 0;
                return;
            }
            playerMoveTime += Time.deltaTime;
            if (playerMoveTime >= Parameters.player_move_delay)
            {
                playerMoveTime = 0;
                if (PlayerCanMove())
                {
                    playerGridPos += player.GetMoveDirection() * new Vector2(1, -1);
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

        private bool PlayerCanMove() 
        {
            Vector2 temp = playerGridPos + player.GetMoveDirection() * new Vector2(1, -1);
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
