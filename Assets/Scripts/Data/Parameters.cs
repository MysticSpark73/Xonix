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

        public static readonly Color grid_color_water = new Color(0.02352941f, 0.06666667f, 0.1647059f, 1);
        public static readonly Color grid_color_ground = new Color(0, 0.6313726f, 0.6705883f, 1);
        public static readonly Color grid_color_trail = new Color(1, 0, 0.7529413f, 1);

        #endregion
        #region Player

        public static readonly float player_move_delay = .05f;
        public static int player_hp { get; private set; } = 3;

        public static void LoseLife() => Mathf.Max(--player_hp, 0);

        #endregion
        #region Enemies

        public static readonly float enemies_move_delay = .05f;

        #endregion
        #region level

        public static readonly float level_target_fill = .75f;

        public static float current_fill_amount { get; private set; } = 0;

        public static int level_score { get; private set; } = 0;
        public static int level_time_seconds { get; private set; } = 60;
        public static int level_games_played { get; private set; } = 0;

        public static void SetFillAmount(float value) => current_fill_amount = value;

        #endregion

    }
}

