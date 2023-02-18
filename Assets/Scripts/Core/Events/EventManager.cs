using System;
using UnityEngine;

namespace Xonix.Core.Events
{
    public static class EventManager
    {
        public static Action<Data.GameState> GameStateChanged;
        public static Action<Vector2> Swipe;
        public static Action PlayerTakenDamage;
        public static Action FillChanged;
        public static Action ScoreChanged;
        public static Action TimeChanged;
    }
}
