using System;
using Enemy;
using UnityEngine;

namespace Abilities
{
    public class AoEBehaviour : MonoBehaviour
    {
        [SerializeField] private float damage = 3f;
        [SerializeField] private float impactForce = 10f;
        [SerializeField] private float aoeMaxScale = 0.3f;
        [HideInInspector] public bool isPlayer1;
        private string _enemyTag;
        private SpriteRenderer _sprite;
        private float _aoeScale;
    
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }
    
        private void Start()
        {
            _enemyTag = isPlayer1 ? "P1 Enemy" : "P2 Enemy";
            
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
            if (_aoeScale < aoeMaxScale)
            {
                _aoeScale += Time.fixedDeltaTime;
                transform.localScale = new Vector3(_aoeScale, _aoeScale, 0f);
            
                var temporaryColor = _sprite.color;
                temporaryColor.a -= 1 / (aoeMaxScale / Time.fixedDeltaTime);
                _sprite.color = temporaryColor;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.gameObject.CompareTag(_enemyTag)) return;
            
            var impact = collision.gameObject.transform.position - transform.position;
            collision.gameObject.GetComponent<EnemyBehaviour>().GetHit(impact, impactForce, damage);
        }
    }
}