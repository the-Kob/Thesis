using System;
using Enemy;
using UnityEngine;

namespace Abilities
{
    public class EffectBehaviour : MonoBehaviour
    {
        private enum Effects
        {
            BuffP2Enemy,
            NerfP2Enemy,
            BuffP1Enemy,
            NerfP1Enemy
        }
        
        [SerializeField] private Effects currentEffect;
        private SpriteRenderer _sprite;
        private float _effectRadius;
        private bool _isEffectSet;
        
        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            if(!_isEffectSet) return;

            if (currentEffect == Effects.NerfP1Enemy || currentEffect == Effects.NerfP2Enemy) // Nerf
            {
                if (_effectRadius < 20f)
                {
                    _effectRadius += Time.fixedDeltaTime * 10f;
                    transform.localScale = new Vector3(_effectRadius, _effectRadius, 0);
                    
                    var temporaryColor = _sprite.color;
                    temporaryColor.a -= Time.fixedDeltaTime * 0.5f;
                    _sprite.color = temporaryColor;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
            else // Buff
            {
                if (_effectRadius > 0f)
                {
                    _effectRadius -= Time.fixedDeltaTime * 10f;
                    transform.localScale = new Vector3(_effectRadius, _effectRadius, 0);
                    
                    var temporaryColor = _sprite.color;
                    temporaryColor.a += Time.fixedDeltaTime * 0.5f;
                    _sprite.color = temporaryColor;
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
        
        // Nerf
        private void OnTriggerEnter2D(Collider2D collision)
        {
            switch (currentEffect)
            {
                case Effects.NerfP1Enemy when collision.CompareTag("P1 Enemy"):
                    collision.gameObject.GetComponent<EnemyBehaviour>().Nerf();
                    break;
                case Effects.NerfP2Enemy when collision.CompareTag("P2 Enemy"):
                    collision.gameObject.GetComponent<EnemyBehaviour>().Nerf();
                    break;
                default:
                    return;
            }
        }
        
        // Buff
        private void OnTriggerExit2D(Collider2D collision)
        {
            switch (currentEffect)
            {
                case Effects.BuffP1Enemy when collision.CompareTag("P1 Enemy"):
                    collision.gameObject.GetComponent<EnemyBehaviour>().Buff();
                    break;
                case Effects.BuffP2Enemy when collision.CompareTag("P2 Enemy"):
                    collision.gameObject.GetComponent<EnemyBehaviour>().Buff();
                    break;
                default:
                    return;
            }
        }

        public void SetEffect(int effect)
        {
            currentEffect = (Effects) effect;

            if (currentEffect == Effects.BuffP1Enemy || currentEffect == Effects.NerfP1Enemy)
            {
                ColorUtility.TryParseHtmlString("#FB8F13", out var p1Color);
                _sprite.color = p1Color;
            } else
            {
                ColorUtility.TryParseHtmlString("#315D9A", out var p2Color);
                _sprite.color = p2Color;
            }
            
            if (currentEffect == Effects.BuffP1Enemy || currentEffect == Effects.BuffP2Enemy)
            {
                _effectRadius = 20f;
                transform.localScale = new Vector3(_effectRadius, _effectRadius, 0);
                
                var temporaryColor = _sprite.color;
                temporaryColor.a = 0;
                _sprite.color = temporaryColor;
            }
            
            _isEffectSet = true;
        }
    }
}
