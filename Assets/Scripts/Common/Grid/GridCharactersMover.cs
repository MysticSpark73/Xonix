using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xonix.Common.Characters;
using Xonix.Core;
using Xonix.Data;

namespace Xonix.Common.Grid
{
    public class GridCharactersMover
    {
        private float playerMoveTime;
        private float enemiesMoveTime;

        private Player player;
        private Vector2 playerSpawnPos;
        private List<Enemy> enemies;

        private GridBuilder grid;

        public GridCharactersMover(GridBuilder grd)
        {
            grid = grd;
            enemies = new List<Enemy>();
        }

        #region Setters

        public void SetPlayer(Player plr) => player = plr;

        public void AddEnemy(Enemy en)
        {
            enemies.Add(en);
            en.SetPos(grid.FindEnemySpawnPos(en.EnemyType));
        }

        public void SetPlayerSpawnPos(Vector2 pos) => playerSpawnPos = pos;

        #endregion
        #region Getters

        public Vector2 GetPlayerSpawnPos() => playerSpawnPos;

        public Vector2 GetPlayerPos() => player.GridPos;

        #endregion

        public bool IsEnemySpawnExcluded(Vector2 exc)
        {
            if (exc == playerSpawnPos)
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

        public void MovePlayer()
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
                if (IsCanMove(player.GridPos, player.MoveDirection))
                {
                    player.SetPos(player.GridPos + player.MoveDirection * Parameters.grid_y_flip_vector);
                    grid.AddToTrail(player.GridPos);
                    CheckGroundColor(player.GridPos);
                }
                else
                {
                    player.Stop();
                    grid.ClearTrail(player.GridPos);
                }
            }
        }

        public void MoveEnemies()
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
                enemy.Bounce(FindFlipVector(grid.FindCornerTiles(enemy.GridPos, enemy.MoveDirection, enemy.EnemyType)));
                enemy.SetPos(enemy.GridPos + enemy.MoveDirection * new Vector2(1, -1));
            }
        }

        public void Damage()
        {
            player.TakeDamage();
        }

        public void ResetPlayer() 
        {
            player.SetPos(playerSpawnPos);
        }

        public void RemoveEnemies() 
        {
            foreach (var e in enemies)
            {
                e.Kill();
                GameManager.Instance.ReturnToPool(e);
            }
            enemies.Clear();
        }

        public void CheckKillEnemy(Vector2 point) 
        {
            foreach (var enemy in enemies.ToList())
            {
                if (enemy.GridPos == point)
                {
                    enemies.Remove(enemy);
                    enemy.Kill();
                    GameManager.Instance.ReturnToPool(enemy);
                    Parameters.KillEnemy();
                    break;
                }
            }
        }

        public void OnTakenDamage() 
        {
            player.SetPos(playerSpawnPos);
            grid.ClearTrail(player.GridPos, true);
        }

        private bool IsCanMove(Vector2 gridPos, Vector2 direction)
        {
            Vector2 temp = gridPos + direction * Parameters.grid_y_flip_vector;
            if (temp.x < 0 || temp.y < 0 || temp.x >= grid.GridWidth || temp.y >= grid.GridHeight)
            {
                return false;
            }
            return true;
        }

        private void CheckGroundColor(Vector2 playerGridPos)
        {
            Color color = grid.GetTileColor(playerGridPos);

            if (color == Parameters.grid_color_water)
            {
                grid.SetTileColor(playerGridPos, Parameters.grid_color_trail);
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
                    grid.ClearTrail(playerGridPos);
                }
            }

            if (color == Parameters.grid_color_trail)
            {
                player.TakeDamage();
            }
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
                    return Parameters.grid_y_flip_vector * -1;
                }
                if (cornerTiles[1])
                {
                    return Parameters.grid_y_flip_vector;
                }
                return Vector2.one * -1;
            }
            return Vector2.one;
        }

    }
}
