using System;
using UnityEngine;

namespace Xonix.Core.Events
{
    public static class EventManager
    {
        public static Action<Data.GameState> GameStateChanged;

        public static Action<Vector2> Swipe;

        public static Action TookDamage;
    }
}
