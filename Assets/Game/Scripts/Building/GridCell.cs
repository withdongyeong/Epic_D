using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 인벤토리 그리드의 개별 셀 관리
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        public Image background;
        public int x;
        public int y;
        public bool isOccupied = false;
        
        [Header("하이라이트 색상")]
        public Color defaultColor = new Color(1f, 1f, 1f, 0.3f);
        public Color hoverColor = new Color(0.8f, 0.8f, 1f, 0.5f);
        public Color occupiedColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        public Color invalidColor = new Color(1.0f, 0.5f, 0.5f, 0.6f);
        public Color validPlacementColor = new Color(0.5f, 1f, 0.5f, 0.6f);
        
        private void Awake()
        {
            if (background == null)
                background = GetComponent<Image>();
                
            background.color = defaultColor;
        }
        
        /// <summary>
        /// 셀 위치 설정
        /// </summary>
        public void SetPosition(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }
        
        /// <summary>
        /// 셀 점유 상태 설정
        /// </summary>
        public void SetOccupied(bool occupied)
        {
            isOccupied = occupied;
            background.color = occupied ? occupiedColor : defaultColor;
        }
        
        /// <summary>
        /// 마우스 오버 시 하이라이트
        /// </summary>
        public void Highlight(bool isHighlighted)
        {
            if (!isOccupied)
            {
                background.color = isHighlighted ? hoverColor : defaultColor;
            }
        }
        
        /// <summary>
        /// 배치 가능 여부에 따른 하이라이트
        /// </summary>
        public void MarkInvalid(bool invalid)
        {
            if (isOccupied)
                return;
                
            if (invalid)
                background.color = invalidColor;
            else
                background.color = validPlacementColor;
        }
        
        /// <summary>
        /// 하이라이트 리셋
        /// </summary>
        public void ResetHighlight()
        {
            background.color = isOccupied ? occupiedColor : defaultColor;
        }
    }
}