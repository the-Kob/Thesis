using UnityEngine;

namespace Background
{
    public class BackgroundTileBehaviour : MonoBehaviour
    {
        private  Camera mainCamera;
        private Rect cameraView;

        private void Start()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        private void Update()
        {
            cameraView = GetCameraView();
            
            if (!IsTileInView())
            {
                Destroy(gameObject);
            }
        }

        private Rect GetCameraView()
        {
            var height = 2f * mainCamera.orthographicSize;
            var width = height * mainCamera.aspect;

            return new Rect((Vector2)mainCamera.transform.position - new Vector2(width / 2, height / 2), new Vector2(width, height));
        }

        private bool IsTileInView()
        {
            return cameraView.Contains(transform.position);
        }
    }   
}