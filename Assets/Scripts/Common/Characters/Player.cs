using UnityEngine;
using UnityEngine.UI;
using Xonix.Core.Events;
using Xonix.Data;

namespace Xonix.Common.Characters
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private RectTransform self;
        [SerializeField] private Sprite walkSprite;
        [SerializeField] private Sprite swimSprite;

        private Vector2 moveDirection;
        private Vector2 spawnPos;
        private bool isSwiming;

        public void Init(Vector2 gridPos) 
        {
            spawnPos = GridPosToAnchor(gridPos);
            self.anchoredPosition = spawnPos;
            image.sprite = walkSprite;
            Subscribe();
        }

        public void Subscribe() 
        {
            EventManager.Swipe += OnSwipe;
        }

        public void UnSubscribe() 
        {
            EventManager.Swipe -= OnSwipe;
        }

        public Vector2 GetMoveDirection() => moveDirection;

        public bool GetIsSwiming() => isSwiming;

        public void SetIsSwiming(bool value) => isSwiming = value;

        public void SetPos(Vector2 gridPos) 
        {
            self.anchoredPosition = GridPosToAnchor(gridPos);
        }

        public void Stop() 
        {
            moveDirection = Vector2.zero;
            isSwiming = false;
        }

        public void TakeDamage() 
        {
            Stop();
            Parameters.LoseLife();
            EventManager.TookDamage?.Invoke();
            if (Parameters.player_hp == 0)
            {
                Application.Quit();
            }
        }

        private void OnApplicationQuit()
        {
            UnSubscribe();
        }

        private void OnSwipe(Vector2 swipe)
        {
            moveDirection = new Vector2(swipe.x, swipe.y);
        }

        private Vector2 GridPosToAnchor(Vector2 gridIndex) 
        {
            return gridIndex * new Vector2(1, -1) * Parameters.grid_tile_size + Parameters.grid_tile_offset;
        }

    }
}