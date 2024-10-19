using System;
using UnityEngine;

public class BackgroundCanvasScaler : MonoBehaviour
{
    private Canvas _backgroundCanvas;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float baseScaleFactor = 1f;
    private Vector3 _initialScale;
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
    }

    private void Update()
    {
        if (!_isSetupValid) return;
        
        var scaleMultiplier = mainCamera.orthographicSize * baseScaleFactor;
        _backgroundCanvas.transform.localScale = _initialScale * scaleMultiplier;
    }
}