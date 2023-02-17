using System;

namespace Xonix.Core.Events
{
    public static class EventManager
    {
        public static Action<Data.GameState> OnGameStateChanged;
    }
}
