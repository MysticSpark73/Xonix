using UnityEngine;
using Xonix.Common.Characters;

namespace Xonix.Common.Grid
{
    public class GridPlayerController
    {
        GridBuilder gridBuilder;
        Player player;
        public GridPlayerController(GridBuilder builder) 
        {
            gridBuilder = builder;
            OnCreate();
        }

        public void SetPlayer(Player player) => this.player = player;

        private void OnCreate() 
        {
            
        }
        
    }
}
