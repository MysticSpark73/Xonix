using UnityEngine;
using UnityEngine.UI;

namespace Xonix.Common.Grid
{
    [RequireComponent(typeof(Image))]
    public class Tile : MonoBehaviour
    {
        private Image image;
        private RectTransform rect;
        private int size;
        public void Init() 
        {
            image = GetComponent<Image>();
            rect = image.rectTransform;
            rect.anchorMin = Vector2.up;
            rect.anchorMax = Vector2.up;
        }

        public void SetColor(Color color) => image.color = color;

        public void SetSize(int size)
        {
            rect.sizeDelta = Vector2.one * size;
            this.size = size;
        }

        public void setGridPos(int x, int y) 
        {
            rect.anchoredPosition = new Vector2(x * size + size/2, -y * size - size/2);
        }

        public Color GetColor() => image.color;

    }
}
