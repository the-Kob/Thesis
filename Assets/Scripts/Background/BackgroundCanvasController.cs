using System;
using UnityEngine;

namespace Background
{
    public class BackgroundCanvasController : MonoBehaviour
    {
        private Canvas _backgroundCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private float zoomScalingFactor = 0.1f;
        private Vector3 _initialScale;
        private float _initialCameraSize;
        private bool _isSetupValid;

        private void Awake()
        {
            _backgroundCanvas = gameObject.GetComponent<Canvas>();
        }

        private void Start()
        {
            _isSetupValid = mainCamera != null && _backgroundCanvas != null;

            if (!_isSetupValid) return;

            _initialScale = _backgroundCanvas.transform.localScale;
            _initialCameraSize = mainCamera.orthographicSize;
        }

        private void Update()
        {
            if (!_isSetupValid) return;
        
            var zoomFactor = Mathf.Pow(_initialCameraSize / mainCamera.orthographicSize, zoomScalingFactor);
        
            _backgroundCanvas.transform.localScale = _initialScale * zoomFactor;
        }
    }
}