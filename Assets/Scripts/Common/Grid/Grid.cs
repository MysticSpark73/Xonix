using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xonix.Data;

namespace Xonix.Common.Grid
{
    public class Grid : MonoBehaviour
    {
        [SerializeField] private Image palyArea;
        [SerializeField] private RectTransform tilesContainer;

        private List<List<Tile>> tileMap;

        public void CreateGrid() 
        {
            int horizontalTilesCount = (int) (palyArea.rectTransform.rect.width / Parameters.grid_tile_size);
            int verticalTilesCount = (int) (palyArea.rectTransform.rect.height / Parameters.grid_tile_size);

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
                    if (i <= 1 || i >= tileMap.Count - 2 || j <= 1 || j >= tileMap[i].Count -2)
                    {
                        tileMap[i][j].SetColor(Parameters.grid_color_ground);
                    }
                }
            }
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

    }
}
