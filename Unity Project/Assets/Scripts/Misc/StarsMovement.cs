using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Misc
{
    public class StarsMovement : MonoBehaviour
    {
        [SerializeField] private bool isPlayer1;
        [SerializeField] private float speed;
        private RectTransform _transform;
        
        private void Awake()
        {
            Random.InitState(42);
            
            _transform = GetComponent<RectTransform>();
        }

       
        private void FixedUpdate()
        {
            if (isPlayer1)
            {
                _transform.anchoredPosition += new Vector2(0.08f + speed, 0f);

                if (_transform.anchoredPosition.x < 590f / 2f) return;
                
                _transform.anchoredPosition = - new Vector2(855f, 0f);
            }
            else
            {
                _transform.anchoredPosition -= new Vector2(0.08f + speed, 0f);
                
                if (_transform.anchoredPosition.x > -590f / 2f) return;
                
                _transform.anchoredPosition = new Vector2(855f, 0f);
            }
        }

        private void OnEnable()
        {
            _transform = GetComponent<RectTransform>();
            _transform.anchoredPosition = new Vector2(_transform.anchoredPosition.x, Random.Range(-115f, 115f));
        }
    }
}

