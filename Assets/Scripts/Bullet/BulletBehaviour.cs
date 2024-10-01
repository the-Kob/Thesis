 using System;
using System.Net.Mail;
using Enemy;
using UnityEngine;

namespace Bullet
{
    public class BulletBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject bulletEffectPrefab;
    [SerializeField] private float effectCooldown = 0.02f;
    [SerializeField] private float angleEffect = 10f;

    [SerializeField] private float lifespan = 5f;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float damage = 5f;
    [SerializeField] private float impactForce = 5f;

    [HideInInspector] public bool isPlayer1;
    
    private SpriteRenderer _sprite;

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
        HandleLifespan();
        
        transform.position += transform.right * (Time.fixedDeltaTime * speed);
        
        HandleBulletEffect();
    }

    private void HandleLifespan()
    {
        if (lifespan < 0f){
            if (isPlayer1){
                // interfaceManager.saveMissPlayer();
            } 
            else 
            {
                // interfaceManager.saveMissNPC();
            }
            
            Destroy(gameObject);
        } else 
        {
            lifespan -= Time.fixedDeltaTime;
        }
    }

    private void HandleBulletEffect()
    {
        if (effectCooldown < 0f){
            var bulletEffectInstance = Instantiate(bulletEffectPrefab, transform.position, Quaternion.Euler(0f, 0f, angleEffect));
            bulletEffectInstance.GetComponent<BulletParticleBehaviour>().isPlayer1 = isPlayer1;
            
            effectCooldown = 0.02f;
            angleEffect = -angleEffect;
        } else 
        {
            effectCooldown -= Time.fixedDeltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isPlayer1 && collision.gameObject.CompareTag("P1 Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().GetHit(transform.right, impactForce, damage);
            Destroy(gameObject);
        } else if (!isPlayer1 && collision.gameObject.CompareTag("P2 Enemy"))
        {
            collision.gameObject.GetComponent<EnemyBehaviour>().GetHit(transform.right, impactForce, damage);
            Destroy(gameObject);
        }
    }
}
}