using UnityEngine;

namespace Bullet
{
    public class BulletParticleBehaviour : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private float _scalar = 1f;
    
        [HideInInspector] public bool isPlayer1;
    
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }
    
        private void Start()
        {
            if (isPlayer1)
            {
                ColorUtility.TryParseHtmlString("#FB8F13", out var p1Color);
                _sprite.color = p1Color;
            } else
            {
                ColorUtility.TryParseHtmlString("#315D9A", out var p2Color);
                _sprite.color = p2Color;
            }
        }
    
        private void FixedUpdate()
        {
            if (_scalar <= 0f)
            {
                Destroy(gameObject);
            } else
            {
                transform.localScale *= _scalar;
                _scalar -= Time.fixedDeltaTime;
            
                var temporaryColor = _sprite.color;
                temporaryColor.a = _scalar;
                _sprite.color = temporaryColor;
            }
        }
    }
}