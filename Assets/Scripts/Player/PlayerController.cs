using System;
using Abilities;
using Bullet;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using ColorUtility = UnityEngine.ColorUtility;

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

        [SerializeField] private GameObject effect;
        [SerializeField] private float effectMenuCooldown;
        private float _effectMenuCooldownTimer;
        private bool _isInEffectMenu;
        [SerializeField] private float chooseEffectCooldown = 10f;
        private float _chooseEffectCooldownTimer;
        private bool _isEffectActive;
        private bool _chooseEffectDownInput;
        private bool _chooseEffectUpInput;
        private int _chosenEffect = -1;
        
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
    
        private void Update()
        {
            HandleLook();
            HandleFire();
            HandleAoE();
            HandleChooseEffect();
        }

        private void FixedUpdate()
        {
            UpdateCooldowns();
            HandleMove();
        }

        private void UpdateCooldowns()
        {
            if (actCooldownTimer > 0f)
            {
                actCooldownTimer -= Time.fixedDeltaTime;
            }
            else if(!canAct)
            {
                canAct = true;
            }

            if (_fireCooldownTimer > 0f)
            {
                _fireCooldownTimer -= Time.fixedDeltaTime;
            }
            else
            {
                _canFire = true;
            }

            if (_aoeCooldownTimer > 0f)
            {
                _aoeCooldownTimer -= Time.fixedDeltaTime;
            }
            
            if (_aoeCooldownTimer < 0f)
            {
                _aoeCooldownTimer = 0f;
            }

            if (_chooseEffectCooldownTimer > 0f)
            {
                _chooseEffectCooldownTimer -= Time.fixedDeltaTime;
            }
            else
            {
                _isEffectActive = true;
            }

            if (_effectMenuCooldownTimer > 0f)
            {
                _effectMenuCooldownTimer -= Time.fixedDeltaTime;
            }
        }

        private void HandleMove()
        {
            Vector3 movement = _hitByEnemy ? _knockbackDirection * (knockbackForce * Time.fixedDeltaTime) : 
                new Vector3(_moveInput.x, _moveInput.y, 0f) * (moveSpeed * Time.fixedDeltaTime);

            if (_hitByEnemy && (_knockbackCooldownTimer -= Time.fixedDeltaTime) <= 0)
                _hitByEnemy = false;

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
                    SetCrosshairVisibility(true);
                }

                UpdateWeaponRotation();
                crosshair.transform.position = (Vector2)transform.position + _lookInput * crosshairRange;
            }
            else
            {
                if (IsCrosshairVisible())
                {
                    SetCrosshairVisibility(false);
                }
            }
        }

        private bool IsCrosshairVisible()
        {
            return _crosshairSprite.color.a > 0;
        }
        
        private void SetCrosshairVisibility(bool visible)
        {
            var temporaryColor = _crosshairSprite.color;
            temporaryColor.a = visible ? 1f : 0f;
            _crosshairSprite.color = temporaryColor;
        }

        private void UpdateWeaponRotation()
        {
            var angle = Vector2.Angle(_lookInput, Vector2.right);

            weapon.transform.rotation = Quaternion.Euler(0f, 0f, _lookInput.y > 0f ? angle : 360f - angle);
        }

        private void HandleFire()
        {
            if(_fireInput == 0 || !_canFire || !canAct) return;
            
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

        private void HandleAoE()
        {
            if (_aoeCooldownTimer > 0f || !_aoeInput || !canAct) return;
            
            _aoeCooldownTimer = aoeCooldown;
            var aoeInstance = Instantiate(aoe, transform.position, Quaternion.identity);
            aoeInstance.GetComponent<AoEBehaviour>().isPlayer1 = isPlayer1;
            UIManager.Instance.TriggerAoE(isPlayer1, aoeCooldown);
        }

        private void HandleChooseEffect()
        {
            HandleChooseEffectDown();
            HandleChooseEffectUp();
        }

        private void HandleChooseEffectDown()
        {
            if (!_chooseEffectDownInput || _isEffectActive) return;

            if (_isInEffectMenu)
            {
                HandleEffectMenuNavigation();
            }
            else if(!_isEffectActive && _effectMenuCooldownTimer <= 0f)
            {
                _isInEffectMenu = true;
                // UIManager.OpenEffectMenu(isPlayer1);
            }
        }

        private void HandleEffectMenuNavigation()
        {
            if (_lookInput == Vector2.zero) return;

            var angle = Vector2.Angle(_lookInput, Vector2.right);

            if (_lookInput.x > 0f)
            {
                    
            }
            else
            {
                    
            }
        }

        private void HandleChooseEffectUp()
        {
            if (!_chooseEffectUpInput || !_isInEffectMenu) return;
            
            _isInEffectMenu = false;
            _effectMenuCooldownTimer = 1f;

            if (_chosenEffect == -1) return;
            
            var effectInstance = Instantiate(effect, transform.position, Quaternion.identity);
            effectInstance.GetComponent<EffectBehaviour>().SetEffect(_chosenEffect);
            _isEffectActive = true;
            UIManager.Instance.TriggerChooseEffect(isPlayer1, chooseEffectCooldown);
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

        public void OnChooseEffect(InputAction.CallbackContext context)
        {
            _chooseEffectDownInput = context.started;
            _chooseEffectUpInput = context.canceled;
        }
    }
}