using System;
using UnityEngine;

namespace Player
{
    public class PlayerCollisionManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        
        private bool _updated;
        private bool _hitEnemy = false;
        [SerializeField] private float hitEffectTime = 3f;
        private float _hitTimer = 0f;

        private void FixedUpdate()
        {
            if (!_hitEnemy) return;
            
            _hitTimer -= Time.fixedDeltaTime;

            if (_hitTimer > 0f || _updated) return;
    
            _hitEnemy = false;
            UIManager.Instance.SetScoreGainAvailability(true);
            _updated = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playerController.isPlayer1 && collision.gameObject.CompareTag("P1 Enemy") && !_hitEnemy)
            {
                HandlePlayerHit(collision.transform.position, true);
            }
            
            if (!playerController.isPlayer1 && collision.gameObject.CompareTag("P2 Enemy") && !_hitEnemy)
            {
                HandlePlayerHit(collision.transform.position, false);
            }
        }
        
        private void HandlePlayerHit(Vector2 enemyPosition, bool isPlayer1)
        {
            _hitEnemy = true;
            _updated = false;
            _hitTimer = hitEffectTime;
            
            playerController.GetHitByEnemy(enemyPosition, hitEffectTime);
            UIManager.Instance.ChangeCombo(false);
            UIManager.Instance.SetScoreGainAvailability(false, isPlayer1);
            playerController.actCooldownTimer = hitEffectTime;
            // bulletManager.setDeadTimer(hitTime);  // Also inform BulletManager about the hit state
        }
    }
}