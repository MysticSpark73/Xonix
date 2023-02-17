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
            EventManager.OnGameStateChanged?.Invoke(GameState);
        }

        public static async void WaitUntilGameState(GameState state) 
        {
            await new WaitUntil(() => GameState == state);
        }

        #endregion
        #region Grid
        
        public static readonly int grid_tile_size = 32;

        public static readonly Color grid_color_water = new Color(0.02352941f, 0.06666667f, 0.1647059f, 1);
        public static readonly Color grid_color_ground = new Color(0, 0.6313726f, 0.6705883f, 1);
        public static readonly Color grid_color_trail = new Color(1, 0, 0.7529413f, 1);


        #endregion

    }
}

