using System.Linq;
using UnityEngine;

namespace Background
{
    public class BackgroundTileBehaviour : MonoBehaviour
    {
        private Camera _mainCamera;
        private RectTransform _rectTransform;
        private float _margin;
        private BackgroundTileManager _tileManager;
        private Vector2Int _tilePos;
        
        public void Initialize(BackgroundTileManager manager, Vector2Int position, float margin)
        {
            _tileManager = manager;
            _tilePos = position;
            _margin = margin;
        }
        
        private void Start()
        {
            if (_mainCamera == null)
            {
                _mainCamera = Camera.main;
            }
            
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
            }
        }

        void Update()
        {
            if (IsTileInView()) return;

            _tileManager.RemoveTile(_tilePos);
            Destroy(gameObject);
        }

        private bool IsTileInView()
        {
            Vector3[] corners = new Vector3[4];
            _rectTransform.GetWorldCorners(corners);
            
            var marginX = _margin * _rectTransform.rect.width;
            var marginY = _margin * _rectTransform.rect.height;

            var screenLeft = 0 - marginX;
            var screenRight = Screen.width + marginX;
            var screenBottom = 0 - marginY;
            var screenTop = Screen.height + marginY;
            
            return corners.Any(corner =>
            {
                Vector3 screenPoint = RectTransformUtility.WorldToScreenPoint(_mainCamera, corner);
                return screenPoint.x > screenLeft && screenPoint.x < screenRight &&
                       screenPoint.y > screenBottom && screenPoint.y < screenTop;
            });
        }
    }   
}