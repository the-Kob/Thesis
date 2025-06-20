using System;
using Abilities;
using Bullet;
using Data_Storage.Events;
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
        private PlayerController _otherPlayer;
        
        public bool HasMoved { get; private set; }
        public bool HasAimed { get; private set; }
        public bool HasFired { get; private set; }
        public bool HasAoEed { get; private set; }
        
        [SerializeField] private GameObject body;
    
        [SerializeField] private float moveSpeed = 5f;
        public Vector2 MoveInput {get; private set;}
    
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
        [HideInInspector] public bool canFire;
        private float _fireCooldownTimer;

        [SerializeField] private GameObject aoe;
        [SerializeField] private float aoeCooldown = 5f;
        private bool _aoeInput;
        private float _aoeCooldownTimer;

        [SerializeField] private GameObject effect;
        [HideInInspector] public bool isInEffectMenu;
        [SerializeField] private float chooseEffectCooldown = 10f;
        private float _chooseEffectCooldownTimer;
        private bool _isEffectActive;
        private bool _chooseEffectDownInput;
        private bool _chooseEffectUpInput;
        private int _chosenEffect = -1;
        private float _effectMenuDebounceTimer;
        private const float DebounceDuration = 0.05f;
        
        public float Distance {get; private set;}
        private float _lastKnownDistanceBetweenPlayers;
        private DistanceTrend _distanceTrend = DistanceTrend.None;
        private MovementTrend _movementTrend = MovementTrend.None;
        private Vector2 _lastTriggerPosition;
        private Vector2 _lastDirection;
        private float _distanceSinceLastTrigger;
        [SerializeField] private float movementThreshold = 2f;
        [SerializeField] private float directionChangeThreshold = 0.5f;
        
        public static event Action OnSubmitPressed;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();

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
            _playerInput.actions["Submit"].performed += OnSubmit;
            
            if (isPlayer1)
            {
                ColorUtility.TryParseHtmlString("#FB8F13", out var p1Color);
                _weaponSprite.color = p1Color;
                
                _playerInput.SwitchCurrentControlScheme(GameManager.Instance.P1Device);
                _otherPlayer = GameObject.FindWithTag("P2").GetComponent<PlayerController>();
            } else
            {
                ColorUtility.TryParseHtmlString("#315D9A", out var p2Color);
                _weaponSprite.color = p2Color;
                
                _playerInput.SwitchCurrentControlScheme(GameManager.Instance.P2Device);
                _otherPlayer = GameObject.FindWithTag("P1").GetComponent<PlayerController>();
            }
            
            _lastTriggerPosition = transform.position;
            _lastDirection = Vector2.zero;
            _lastKnownDistanceBetweenPlayers = Vector2.Distance(transform.position, _otherPlayer.transform.position);
        }
    
        private void Update()
        {
            UpdateCooldowns();
            HandleLook();
            HandleFire();
            HandleAoE();
            HandleChooseEffect();
        }

        private void FixedUpdate()
        {
            HandleMove();
            CheckForDisplacementTrigger();
        }
        
        private void CheckForDisplacementTrigger()
        {
            if (_otherPlayer == null) return;

            var currentPosition = new Vector2(transform.position.x, transform.position.y);
            var displacement = currentPosition - _lastTriggerPosition;
            _distanceSinceLastTrigger = displacement.magnitude;

            if (_distanceSinceLastTrigger < movementThreshold) return;

            var currentDirection = displacement.normalized;

            // Only proceed if direction changed significantly
            if (Vector2.Dot(currentDirection, _lastDirection) > directionChangeThreshold) return;
            
            AnalyzeRelativeMovement();
            TriggerDisplacementEvent(currentPosition);

            // Update tracking state
            _lastDirection = currentDirection;
            _lastTriggerPosition = currentPosition;
            _distanceSinceLastTrigger = 0f;
        }
        
        private void AnalyzeRelativeMovement()
        {
            Vector2 directionToOtherPlayer = (_otherPlayer.transform.position - transform.position).normalized;
            var dot = Vector2.Dot(MoveInput.normalized, directionToOtherPlayer);

            _movementTrend = dot switch
            {
                > 0.5f => MovementTrend.Toward,
                < -0.5f => MovementTrend.Away,
                _ => MovementTrend.Sideways
            };
        }
        
        private void TriggerDisplacementEvent(Vector2 currentPosition)
        {
            if (_otherPlayer == null) return;

            Distance = Vector2.Distance(currentPosition, _otherPlayer.transform.position);

            _distanceTrend = Distance < _lastKnownDistanceBetweenPlayers
                ? DistanceTrend.Closer
                : Distance > _lastKnownDistanceBetweenPlayers
                    ? DistanceTrend.Farther
                    : DistanceTrend.Same;

            _lastKnownDistanceBetweenPlayers = Distance;
            
            /*
             * Why are both trends important?
             *
             * The Movement trend represents the player's intention to move toward, away from, or alongside the other player.
             * It tells us what the player *wants* to do, but this may not always align with the actual outcome.
             *
             * Distance trend represents reality: Is the player actually getting farther away? Closer? Or is the distance unaffected?
             * This trend provides an additional dimension that the movement trend doesn't capture.
             */
            UIManager.Instance?.TriggerPlayerDisplacement(
                isPlayer1,
                _lastKnownDistanceBetweenPlayers,
                _distanceSinceLastTrigger,
                _distanceTrend,
                _movementTrend
            );
        }

        private void UpdateCooldowns()
        {
            if (actCooldownTimer > 0f)
            {
                actCooldownTimer -= Time.deltaTime;
            }
            else if(!canAct)
            {
                canAct = true;
            }

            if (_fireCooldownTimer > 0f)
            {
                _fireCooldownTimer -= Time.deltaTime;
            }
            else
            {
                canFire = true;
            }

            if (_aoeCooldownTimer > 0f)
            {
                _aoeCooldownTimer -= Time.deltaTime;
            }
            
            if (_aoeCooldownTimer < 0f)
            {
                _aoeCooldownTimer = 0f;
            }

            if (_chooseEffectCooldownTimer > 0f)
            {
                _chooseEffectCooldownTimer -= Time.deltaTime;
            } 
            
            if(_chooseEffectCooldownTimer < 0f) 
            {
                _isEffectActive = false;
            }

            if (_effectMenuDebounceTimer > 0f)
            {
                _effectMenuDebounceTimer -= Time.deltaTime;
            }
        }

        private void HandleMove()
        {
            Vector3 movement = _hitByEnemy ? _knockbackDirection * (knockbackForce * Time.fixedDeltaTime) : 
                new Vector3(MoveInput.x, MoveInput.y, 0f) * (moveSpeed * Time.fixedDeltaTime);

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
            
            if (_isLooking && !isInEffectMenu)
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
            if(_fireInput == 0 || !canFire || !canAct) return;
            
            canFire = false;
            
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
            UIManager.Instance?.TriggerAoE(isPlayer1, aoeCooldown, Distance);
        }

        private void HandleChooseEffect()
        {
            HandleChooseEffectDown();
            HandleEffectMenuNavigation();
            HandleChooseEffectUp();
        }

        private void HandleChooseEffectDown()
        {   
            if (!_chooseEffectDownInput || _isEffectActive || _effectMenuDebounceTimer > 0f) return;
            
            _chooseEffectDownInput = false;
            _effectMenuDebounceTimer = DebounceDuration;
            
            isInEffectMenu = true;
            UIManager.Instance?.OpenEffectMenu(isPlayer1);
        }

        private void HandleEffectMenuNavigation()
        {
            if(!isInEffectMenu) return;

            if (_lookInput != Vector2.zero)
            {
                var angle = Mathf.Atan2(_lookInput.y, _lookInput.x) * Mathf.Rad2Deg;
                if (angle < 0) angle += 360f;
                
                _chosenEffect = angle switch
                {
                    >= 45f and < 135f when _chosenEffect != 2 => 2,  // Up
                    >= 135f and < 225f when _chosenEffect != 1 => 1,  // Left
                    >= 225f and < 315f when _chosenEffect != 3 => 3,  // Down
                    >= 0f and < 45f or >= 315f and <= 360f when _chosenEffect != 0 => 0, // Right
                    _ => _chosenEffect
                };
            
                UIManager.Instance?.ClearEffectChoice(isPlayer1);
                UIManager.Instance?.ChooseEffect(isPlayer1, _chosenEffect);
            }
            
            UIManager.Instance?.ScaleEffectButtons(isPlayer1, _chosenEffect);
        }

        private void HandleChooseEffectUp()
        {
            if (!_chooseEffectUpInput || !isInEffectMenu) return;
            
            _chooseEffectUpInput = false;
            
            isInEffectMenu = false;
            
            UIManager.Instance?.CloseEffectMenu(isPlayer1, _chosenEffect);
            
            if (_chosenEffect == -1) return;
            
            var effectInstance = Instantiate(effect, transform.position, Quaternion.identity);
            effectInstance.GetComponent<EffectBehaviour>().SetEffect(_chosenEffect);
            _isEffectActive = true;
            _chooseEffectCooldownTimer = chooseEffectCooldown;
            UIManager.Instance?.TriggerChooseEffect(isPlayer1, chooseEffectCooldown, _chosenEffect, Distance);

            if (TutorialManager.Instance != null)
            {
                TutorialManager.Instance.GetEffectFromPlayer(isPlayer1, _chosenEffect);
            }

            _chosenEffect = -1;
        }
        
        public void OnMove(InputAction.CallbackContext context)
        {
            MoveInput = context.ReadValue<Vector2>();
            
            if (TutorialManager.Instance != null && TutorialManager.Instance.currentTutorialStep == 0 && MoveInput.magnitude > 0.1f)
            {
                HasMoved = true;
            }
        }
        
        public void OnLook(InputAction.CallbackContext context)
        {
            _lookInput = context.ReadValue<Vector2>();
            
            if (TutorialManager.Instance != null && TutorialManager.Instance.currentTutorialStep == 1 && _lookInput.magnitude > 0.5f)
            {
                HasAimed = true;
            }
        }
        
        public void OnFire(InputAction.CallbackContext context)
        {
            _fireInput = context.ReadValue<float>();
            
            if (TutorialManager.Instance != null && TutorialManager.Instance.currentTutorialStep == 2 && _fireInput > 0.1f)
            {
                HasFired = true;
            }
        }

        public void OnAoE(InputAction.CallbackContext context)
        {
            _aoeInput = context.performed;
            
            if (TutorialManager.Instance != null && TutorialManager.Instance.currentTutorialStep == 5 && _aoeInput)
            {
                HasAoEed = true;
            }
        }

        public void OnChooseEffect(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                _chooseEffectDownInput = true;
            }

            if (context.canceled)
            {
                _chooseEffectUpInput = true;
            }
        }
        
        private void OnSubmit(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
            
            OnSubmitPressed?.Invoke();
        }

        public void ResetFlags()
        {
            HasMoved = false;
            HasAimed = false;
            HasFired = false;
            HasAoEed = false;
        }
    }
}