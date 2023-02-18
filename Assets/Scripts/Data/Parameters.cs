using UnityEngine;
using Xonix.Core.Events;

namespace Xonix.Data
{
    public static class Parameters
    {
        #region GameState
        public static GameState GameState { get; private set; }

        public static void SwitchGameState(GameState state) 
        {
            GameState = state;
            EventManager.GameStateChanged?.Invoke(GameState);
        }

        public static async void WaitUntilGameState(GameState state) 
        {
            await new WaitUntil(() => GameState == state);
        }

        #endregion
        #region Grid
        
        public static readonly int grid_tile_size = 32;
        public static readonly Vector2 grid_tile_offset = new Vector2(grid_tile_size / 2, -grid_tile_size / 2);
        public static readonly Vector2 grid_y_flip_vector = new Vector2(1, -1);

        public static readonly Color grid_color_water = new Color(0.02352941f, 0.06666667f, 0.1647059f, 1);
        public static readonly Color grid_color_ground = new Color(0, 0.6313726f, 0.6705883f, 1);
        public static readonly Color grid_color_trail = new Color(1, 0, 0.7529413f, 1);

        #endregion
        #region Player

        public static readonly float player_move_delay = .05f;
        public static int player_hp { get; private set; } = 30;

        public static void LoseLife()
        {
            Mathf.Max(--player_hp, 0);
            EventManager.PlayerTakenDamage?.Invoke();
            if (player_hp == 0)
            {
                SwitchGameState(GameState.Ended);
            }
        }

        public static void ResetPlayerHP() 
        {
            player_hp = 3;
        }

        #endregion
        #region Enemies

        public static readonly float enemies_move_delay = .05f;

        #endregion
        #region level

        public static readonly float level_target_fill = .75f;
        public static readonly int kill_enemy_score = 30;

        public static float current_fill_amount { get; private set; } = 0;

        public static int level_score { get; private set; } = 0;
        public static int level_time_seconds { get; private set; } = 60;
        public static int level_games_played { get; private set; } = 0;

        public static void KillEnemy() => AddScore(kill_enemy_score);

        public static void AddScore(int value) 
        {
            level_score += value;
            EventManager.ScoreChanged?.Invoke();
            Debug.Log($"Score = {level_score}, value = {value}");
        }

        public static void ResetScore() 
        {
            level_score = 0;
            EventManager.ScoreChanged?.Invoke();
        }

        public static void SubtractSeconds(int time) 
        {
            level_time_seconds = Mathf.Max(level_time_seconds - time, 0);
            EventManager.TimeChanged?.Invoke();
            if (level_time_seconds == 0)
            {
                SwitchGameState(GameState.Ended);
            }
        }

        public static void ResetTime() 
        {
            level_time_seconds = 60;
            EventManager.TimeChanged?.Invoke();
        }

        public static void SetFillAmount(float value)
        {
            current_fill_amount = value;
            EventManager.FillChanged?.Invoke();
            if (current_fill_amount >= level_target_fill)
            {
                SwitchGameState(GameState.Ended);
                level_games_played++;
                AddScore(level_time_seconds);
            }
        }

        public static void ResetFillAmount() 
        {
            current_fill_amount = 0;
            EventManager.FillChanged?.Invoke();
        }


        #endregion
        #region LevelDialog

        public static readonly string level_dialog_score_text = "SCORE:";
        public static readonly string level_dialog_health_text = "XN:";
        public static readonly string level_dialog_fill_text = "FILL:";
        public static readonly string level_dialog_time_text = "TIME:";

        #endregion

    }
}

