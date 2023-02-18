using UnityEngine;
using UnityEngine.UI;
using Xonix.Data;

namespace Xonix.Common.Characters
{
    public class Enemy : MonoBehaviour
    {
        public EnemyType EnemyType => type;

        public Vector2 GridPos => gridPos;
        public Vector2 MoveDirection => moveDirection;

        [SerializeField] private Image image;
        [SerializeField] private RectTransform self;
        [SerializeField] private Sprite waterEnemySprite;
        [SerializeField] private Sprite groundEnemySprite;

        private EnemyType type;
        private Vector2 moveDirection;
        private Vector2 gridPos;

        public void Init(EnemyType type, RectTransform container) 
        {
            this.type = type;
            switch (type)
            {
                case EnemyType.Water:
                    image.sprite = waterEnemySprite;
                    break;
                case EnemyType.Ground:
                    image.sprite = groundEnemySprite;
                    break;
            }
            self.SetParent(container);
            self.sizeDelta = Vector2.one * Parameters.grid_tile_size;
            moveDirection = RandomizeMoveDirection();
            gameObject.SetActive(true);
        }

        public void SetPos(Vector2 gridPos) 
        {
            this.gridPos = gridPos;
            self.anchoredPosition = GridPosToAnchor(gridPos);
        }

        public void Bounce(Vector2 flip) 
        {
            moveDirection *= flip;
        }

        public void Kill() 
        {
            gameObject.SetActive(false);
            moveDirection = Vector2.zero;
        }

        private Vector2 RandomizeMoveDirection() => new Vector2(Random.Range(0, 2) % 2 == 0 ? 1 : -1, Random.Range(0, 1) % 2 == 0 ? 1 : -1);

        private Vector2 GridPosToAnchor(Vector2 gridIndex) => gridIndex * new Vector2(1, -1) * Parameters.grid_tile_size;

    }
}
