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
            // interfaceManager.setScoreUnable(true);
            _updated = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (playerController.isPlayer1 && collision.gameObject.CompareTag("P1 Enemy") && !_hitEnemy)
            {
                HandlePlayerHit(collision.transform.position, 0);
            }
            
            if (!playerController.isPlayer1 && collision.gameObject.CompareTag("P2 Enemy") && !_hitEnemy)
            {
                HandlePlayerHit(collision.transform.position, 1);
            }
        }
        
        private void HandlePlayerHit(Vector2 enemyPosition, int playerIndex)
        {
            _hitEnemy = true;
            _updated = false;
            _hitTimer = hitEffectTime;
            
            playerController.GetHitByEnemy(enemyPosition, hitEffectTime);
            // interfaceManager.addToCombo(false);  // Reset combo
            // interfaceManager.setScoreUnable(false, playerIndex);  // Disable score for the player
            playerController.actCooldownTimer = hitEffectTime;
            // bulletManager.setDeadTimer(hitTime);  // Also inform BulletManager about the hit state
        }
    }
}