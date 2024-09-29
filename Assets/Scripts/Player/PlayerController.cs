using System;
using Abilities;
using Bullet;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerController : MonoBehaviour
    {
        private PlayerInput _playerInput;
        public bool isPlayer1 = true;

        [SerializeField] private GameObject body;
        private Rigidbody2D _rigidbody;
    
        [SerializeField] private float moveSpeed = 5f;
        private Vector2 _moveInput;
    
        [SerializeField] private float knockbackForce = 5f;
        [SerializeField] private float knockbackCooldown = 0.2f;
        private bool _hitByEnemy;
        private Vector2 _knockbackDirection;
        private float _knockbackCooldownTimer;
        
        [SerializeField] private GameObject crosshair;
        [SerializeField] private float crosshairRange = 2.5f;
        private SpriteRenderer _crosshairSprite;
        
        private Vector2 _lookInput;
        private bool _isLooking;
        
        [HideInInspector] public bool canAct = true;
        [HideInInspector] public float actCooldownTimer;
        
        [SerializeField] private GameObject weapon;
        private SpriteRenderer _weaponSprite;
        [SerializeField] private GameObject bullet;
        [SerializeField] private float fireCooldown = 0.5f;
        private float _fireInput;
        private bool _isFiring;
        private bool _canFire;
        private float _fireCooldownTimer;

        [SerializeField] private GameObject aoe;
        [SerializeField] private float aoeCooldown = 5f;
        private bool _aoeInput;
        private float _aoeCooldownTimer;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            
            if (body != null)
            {
                _rigidbody = body.GetComponent<Rigidbody2D>();
            }

            if (crosshair != null)
            {
                _crosshairSprite = crosshair.GetComponent<SpriteRenderer>();
            }

            if (weapon != null)
            {
                weapon.transform.SetParent(transform);
                _weaponSprite = weapon.GetComponent<SpriteRenderer>();
            }
        }

        private void Start()
        {
            if (isPlayer1)
            {
                ColorUtility.TryParseHtmlString("#FB8F13", out var p1Color);
                _weaponSprite.color = p1Color;
                
                _playerInput.SwitchCurrentControlScheme(GameManager.Instance.P1Device);
            } else
            {
                ColorUtility.TryParseHtmlString("#315D9A", out var p2Color);
                _weaponSprite.color = p2Color;
                
                _playerInput.SwitchCurrentControlScheme(GameManager.Instance.P2Device);
            }
        }
    
        private void FixedUpdate()
        {
            if (actCooldownTimer > 0f)
            {
                actCooldownTimer -= Time.fixedDeltaTime;
            }
            else if(!canAct)
            {
                canAct = true;
            }
            
            HandleMove();
            HandleLook();
            HandleFire();
            HandleAoE();
        }

        private void HandleMove()
        {
            Vector3 movement;
            
            if (_hitByEnemy)
            {
                movement = _knockbackDirection * (knockbackForce * Time.fixedDeltaTime);
        
                _knockbackCooldownTimer -= Time.fixedDeltaTime;
            
                if (_knockbackCooldownTimer <= 0)
                {
                    _hitByEnemy = false;
                }
            }
            else
            {
                movement = new Vector3(_moveInput.x, _moveInput.y, 0f) * (moveSpeed * Time.fixedDeltaTime);
            }
            
            transform.position += movement;
        }

        public void GetHitByEnemy(Vector3 enemyPosition, float cooldown)
        {
            _knockbackDirection = (transform.position - enemyPosition).normalized;
        
            // Trigger knockback
            _hitByEnemy = true;
            _knockbackCooldownTimer = knockbackCooldown;
            
            // Activate act cooldown
            actCooldownTimer = cooldown;
        }

        private void HandleLook()
        {
            _isLooking = _lookInput != Vector2.zero;
            
            if (_isLooking)
            {
                if (!IsCrosshairVisible())
                {
                    SetCrosshairVisibility(1f);
                }

                UpdateWeaponRotation();
                crosshair.transform.position = (Vector2)transform.position + _lookInput * crosshairRange;
            }
            else
            {
                if (IsCrosshairVisible())
                {
                    SetCrosshairVisibility(0f);
                }
            }
        }

        private bool IsCrosshairVisible()
        {
            return _crosshairSprite.color.a > 0;
        }
        
        private void SetCrosshairVisibility(float alpha)
        {
            var temporaryColor = _crosshairSprite.color;
            temporaryColor.a = alpha;
            _crosshairSprite.color = temporaryColor;
        }

        private void UpdateWeaponRotation()
        {
            var angle = Vector2.Angle(_lookInput, Vector2.right);

            weapon.transform.rotation = Quaternion.Euler(0f, 0f, _lookInput.y > 0f ? angle : 360f - angle);
        }

        private void HandleFire()
        {
            _isFiring = _fireInput != 0;

            if (_isFiring && _canFire && canAct)
            {
                _canFire = false;
                
                if (isPlayer1)
                {
                    AudioManager.Instance.PlayP1Shot();
                }
                else
                {
                    AudioManager.Instance.PlayP2Shot(); 
                }

                var bulletInstance = Instantiate(bullet, weapon.transform.position, weapon.transform.rotation);
                bulletInstance.GetComponent<BulletBehaviour>().isPlayer1 = isPlayer1;
                _fireCooldownTimer = fireCooldown;
            }

            if (_fireCooldownTimer > 0f)
            {
                _fireCooldownTimer -= Time.fixedDeltaTime;
            }

            if (_fireCooldownTimer <= 0f)
            {
                _canFire = true;
            }
        }

        private void HandleAoE()
        {
            if (_aoeCooldownTimer <= 0f && _aoeInput && canAct)
            {
                _aoeCooldownTimer = aoeCooldown;
                var aoeInstance = Instantiate(aoe, transform.position, Quaternion.identity);
                aoeInstance.GetComponent<AoEBehaviour>().isPlayer1 = isPlayer1;
                UIManager.Instance.TriggerAoE(isPlayer1, aoeCooldown);
            }

            if (_aoeCooldownTimer <= 0f) return;

            _aoeCooldownTimer -= Time.fixedDeltaTime;

            if (_aoeCooldownTimer < 0f)
            {
                _aoeCooldownTimer = 0f;
            }
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInput = context.ReadValue<Vector2>();
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
        }
        
        public void OnFire(InputAction.CallbackContext context)
        {
            _fireInput = context.ReadValue<float>();
        }

        public void OnAoE(InputAction.CallbackContext context)
        {
            _aoeInput = context.performed;
        }
    }
}