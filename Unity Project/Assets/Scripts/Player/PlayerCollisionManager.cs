using System;
using UnityEngine;

namespace Player
{
    public class PlayerCollisionManager : MonoBehaviour
    {
        [SerializeField] private PlayerController playerController;
        
        private bool _updated;
        private bool _hitEnemy;
        [SerializeField] private float hitEffectTime = 3f;
        private float _hitTimer;

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
                HandlePlayerHit(collision.transform.position, true, playerController.Distance);
            }
            
            if (!playerController.isPlayer1 && collision.gameObject.CompareTag("P2 Enemy") && !_hitEnemy)
            {
                HandlePlayerHit(collision.transform.position, false, playerController.Distance);
            }
        }
        
        private void HandlePlayerHit(Vector2 enemyPosition, bool isPlayer1, float distance)
        {
            _hitEnemy = true;
            _updated = false;
            _hitTimer = hitEffectTime;
            
            playerController.GetHitByEnemy(enemyPosition, hitEffectTime);
            UIManager.Instance.ChangeCombo(false);
            UIManager.Instance.SetScoreGainAvailability(false, isPlayer1, distance);
            playerController.actCooldownTimer = hitEffectTime;
        }
    }
}