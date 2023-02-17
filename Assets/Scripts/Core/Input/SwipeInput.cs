using UnityEngine;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.Core.Input
{
    public class SwipeInput : MonoBehaviour
    {
        private int swipeMinDist = 250;
        private Vector2 startPoint;
        private Vector2 endPoint;
        private SwipeState swipeState;

        void Update()
        {
            if (Parameters.GameState == GameState.Playing)
            {
                CheckSwipe();
            }
            else
            {
                if (swipeState != SwipeState.NoSwipe)
                {
                    ResetSwipe();
                }
            }
        }

        private void CheckSwipe()
        {

#if UNITY_EDITOR
            if (UnityEngine.Input.GetMouseButtonUp(0))
            {
                ResetSwipe();
                return;
            }
            if (UnityEngine.Input.GetMouseButton(0))
            {
                if (swipeState == SwipeState.NoSwipe)
                {
                    swipeState = SwipeState.Begun;
                    startPoint = UnityEngine.Input.mousePosition;
                }
                if (swipeState == SwipeState.Begun)
                {
                    endPoint = UnityEngine.Input.mousePosition;
                    if ((endPoint - startPoint).magnitude >= swipeMinDist)
                    {
                        Debug.DrawLine(endPoint, startPoint, Color.red, 1.0f);
                        SendSwipe(endPoint - startPoint);
                        swipeState = SwipeState.Ended;
                    }
                }
            }
            
#elif PLATFORM_ANDROID

            if (UnityEngine.Input.touchCount == 0)
            {
                ResetSwipe();
                return;
            }
            else
            {
                if (swipeState == SwipeState.NoSwipe)
                {
                    swipeState = SwipeState.Begun;
                    startPoint = UnityEngine.Input.GetTouch(0).position;
                }
                if (swipeState == SwipeState.Begun)
                {
                    endPoint = UnityEngine.Input.GetTouch(0).position;
                    if ((endPoint - startPoint).magnitude >= swipeMinDist)
                    {
                        SendSwipe(endPoint - startPoint);
                        swipeState = SwipeState.Ended;
                    }
                }
            }
#endif
        }

        private void SendSwipe(Vector2 swipeDirection)
        {
            Vector2 dir = swipeDirection.normalized;
            float dot = Vector2.Dot(dir, Vector2.right);
            if (dot >= 0.5f)
            {
                EventManager.Swipe?.Invoke(Vector2.right);
                return;
            }
            else if (dot <= -0.5f)
            {
                EventManager.Swipe?.Invoke(Vector2.left);
                return;
            }
            dot = Vector2.Dot(dir, Vector2.up);
            if (dot >= 0.5f)
            {
                EventManager.Swipe(Vector2.up);
                return;
            }
            else if (dot <= -0.5f)
            {
                EventManager.Swipe(Vector2.down);
                return;
            }
        }

        private void ResetSwipe()
        {
            swipeState = SwipeState.NoSwipe;
            startPoint = Vector2.one * -1;
            endPoint = Vector2.one * -1;
        }
    }
}
