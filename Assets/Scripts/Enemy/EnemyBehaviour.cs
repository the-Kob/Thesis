using System;
using System.Collections;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Enemy
{
    public class EnemyBehaviour : MonoBehaviour
    {
        private static readonly Vector3 LifeBarScale = new Vector3(2.048f, 0.384f, 1f);
        private static readonly float AnimationTime = 25f;
        
        private SpriteRenderer _sprite;
        
        private GameObject _player;
        public bool isPlayer1;

        [SerializeField] private GameObject[] healthLayers;
        [SerializeField] private SpriteRenderer animationHolder;
        [SerializeField] private Sprite hollowLevel;
        [SerializeField] private Sprite fullLevel;
        
        [SerializeField] private float impactCooldown = 0.2f;
        private float _impactStrenght;
        private bool _hitByObject;
        private Vector3 _impactPoint;
        private float _impactCooldownTimer;
        
        [SerializeField] private float movementSpeedMultiplier = 1f;
        [SerializeField] private float getHitMultiplier = 1f;
        private int _health = 2;
        private int _currentHealthLayer = 2;

        private GameObject _lastEnemyHit;
        
        private float _farFromScreen = 1f;
        
        private Vector3 _avoidEnemyVector = Vector3.zero;

        private Sprite _spriteDuringEffect;
        

        private bool _shotDuringBuff;
        private bool _beingBuffed;
        private bool _beingNerfed;
        private Coroutine _buffNerfCoroutine;

        private void Awake()
        {
            _player = isPlayer1 ? GameObject.FindGameObjectWithTag("P1") : GameObject.FindGameObjectWithTag("P2");
            gameObject.tag = isPlayer1 ? "P1 Enemy" : "P2 Enemy";
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
            
            foreach (var healthLayer in healthLayers)
            {
                healthLayer.GetComponent<HealthLayerBehaviour>().InitializeHealthLayer(_sprite.color, isPlayer1);
            }
        }

        private void FixedUpdate()
        {
            HandleMovement();
            HandleGetHit();
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if ((!collision.gameObject.CompareTag("P1 Enemy") || !gameObject.CompareTag("P1 Enemy"))
                && (!collision.gameObject.CompareTag("P2 Enemy") || !gameObject.CompareTag("P2 Enemy"))) return;

            if (!_hitByObject || collision.gameObject == _lastEnemyHit) return;
            
            var enemy = collision.gameObject.GetComponent<EnemyBehaviour>();
            var impactPoint = (-transform.position + collision.transform.position).normalized;
            enemy.GetHit(impactPoint, _impactStrenght / 1.2f, 0f);
            
            _lastEnemyHit = collision.gameObject;
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if ((!collision.gameObject.CompareTag("P1 Enemy") || !gameObject.CompareTag("P1 Enemy"))
                && (!collision.gameObject.CompareTag("P2 Enemy") || !gameObject.CompareTag("P2 Enemy"))) return;

            _avoidEnemyVector -= (transform.position - collision.transform.position).normalized;
        }

        private void HandleMovement()
        {
            if (_player == null) return;
            
            var vectorFromPlayer = (transform.position - _player.transform.position).normalized;

            if (_avoidEnemyVector != Vector3.zero)
            {
                vectorFromPlayer += _avoidEnemyVector;
                vectorFromPlayer = vectorFromPlayer.normalized;
                vectorFromPlayer.z = 0f;
                
                _avoidEnemyVector = Vector3.zero;
            }

            _farFromScreen = (Vector3.Distance(_player.transform.position, transform.position) > 5f)
                ? Mathf.Clamp(Mathf.Pow(Vector3.Distance(transform.position, _player.transform.position) / 10f, 5f), 1f, 5f)
                : 1f;

            transform.position -= Time.fixedDeltaTime * movementSpeedMultiplier * _farFromScreen * vectorFromPlayer;
        }

        private void HandleGetHit()
        {
            if (_hitByObject)
            {
                transform.position += Time.fixedDeltaTime * getHitMultiplier * _impactStrenght * _impactPoint;
                _impactCooldownTimer -= Time.fixedDeltaTime;
            }
            
            if (_impactCooldownTimer > 0) return;

            _hitByObject = false;
        }

        public void GetHit(Vector3 impactPoint, float impactStrenght, float damage)
        {
            if (damage > 0f)
            {
                if (_beingBuffed)
                {
                    animationHolder.sprite = hollowLevel;
                    _shotDuringBuff = true;
                }
                
                if (_health < 0) return;

                healthLayers[_health].GetComponent<SpriteRenderer>().sprite = hollowLevel;
                _health -= 1;
            }

            if (_health < 0)
            {
                for (var i = 0; i < healthLayers.Length; i++)
                {
                    if (i <= _currentHealthLayer)
                    {
                        healthLayers[i].transform.parent = null;
                        healthLayers[i].GetComponent<HealthLayerBehaviour>().SetParameters(i + 1);
                        healthLayers[i].GetComponent<HealthLayerBehaviour>().StartConversionHealthToScore(impactPoint);
                    }
                    else
                    {
                        Destroy(healthLayers[i].gameObject);
                    }
                }
                
                Destroy(gameObject);
            }
            else
            {
                _hitByObject = true;
                _impactCooldownTimer = impactCooldown;
                _impactPoint = impactPoint;
                _impactStrenght = impactStrenght;
            }
        }

        public void Buff()
        {
            if (_currentHealthLayer >= 4) return;

            getHitMultiplier *= 0.75f;
            movementSpeedMultiplier *= 1.1f;
            
            UpdateSprite(true);
        }
        
        public void Nerf()
        {
            if (_currentHealthLayer <= 0) return;
            
            getHitMultiplier /= 0.75f;
            movementSpeedMultiplier /= 1.1f;
            
            UpdateSprite(false);
        }

        private void UpdateSprite(bool isBuff)
        {
            if (_beingBuffed)
            {
                StopCoroutine(_buffNerfCoroutine);
                
                healthLayers[_currentHealthLayer].GetComponent<SpriteRenderer>().sprite = _shotDuringBuff 
                    ? hollowLevel : _spriteDuringEffect;
                _beingBuffed = false;
                _shotDuringBuff = false;
                animationHolder.sprite = null;
            } 
            else if (_beingNerfed)
            {
                StopCoroutine(_buffNerfCoroutine);

                _beingNerfed = false;
                animationHolder.sprite = null;
                animationHolder.transform.localScale = LifeBarScale;
            }

            if (isBuff)
            {
                Sprite newSprite;
                
                if (_health == _currentHealthLayer)
                {
                    _health += 1;
                    newSprite = fullLevel;
                }
                else
                {
                    newSprite = hollowLevel;
                }
                
                _currentHealthLayer += 1;
                
                _beingBuffed = true;
                _buffNerfCoroutine =
                    StartCoroutine(BuffCoroutine(healthLayers[_currentHealthLayer].transform.position, newSprite));
            }
            else
            {
                if (_health == _currentHealthLayer)
                {
                    _health -= 1;
                }
                
                _currentHealthLayer -= 1;
                
                _beingNerfed = true;
                _buffNerfCoroutine = 
                    StartCoroutine(NerfCoroutine(healthLayers[_currentHealthLayer].transform.position, healthLayers[_currentHealthLayer].GetComponent<SpriteRenderer>().sprite));
                healthLayers[_currentHealthLayer].GetComponent<SpriteRenderer>().sprite = null;
            }
        }
        
        private IEnumerator BuffCoroutine(Vector3 position, Sprite newSprite)
        {
            _spriteDuringEffect = newSprite;
            animationHolder.transform.position = position + new Vector3(0f, 0.05f * AnimationTime, 0f);
            animationHolder.sprite = _spriteDuringEffect;
            
            var tmp = animationHolder.color;
            tmp.a = 0f;
            animationHolder.color = tmp;

            var counter = AnimationTime;

            while (counter > 0f)
            {
                counter--;
                animationHolder.transform.position -= new Vector3(0f, 0.05f * AnimationTime, 0f);
                tmp.a += 1f / AnimationTime;
                animationHolder.color = tmp;
                
                yield return new WaitForFixedUpdate();
            }

            healthLayers[_currentHealthLayer].GetComponent<SpriteRenderer>().sprite =
                _shotDuringBuff ? hollowLevel : newSprite;

            _beingBuffed = false;
            _shotDuringBuff = false;
            animationHolder.sprite = null;
            
            yield return null;
        }

        private IEnumerator NerfCoroutine(Vector3 position, Sprite newSprite)
        {
            _spriteDuringEffect = newSprite;
            animationHolder.transform.position = position;
            animationHolder.sprite = _spriteDuringEffect;
            
            var tmp = animationHolder.color;
            tmp.a = 1f;
            animationHolder.color = tmp;
            
            var counter = AnimationTime;

            while (counter > 0f)
            {
                counter--;
                animationHolder.transform.localScale += new Vector3(0.05f, 0.009375f, 0f);
                tmp.a -= 1f / AnimationTime;
                animationHolder.color = tmp;
                
                yield return new WaitForFixedUpdate();
            }

            _beingNerfed = false;
            animationHolder.sprite = null;
            animationHolder.transform.localScale = LifeBarScale;
            
            yield return null;
        }
    }
}
