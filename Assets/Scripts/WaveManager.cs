using System;
using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }
    
    [SerializeField] private PlayerController p1;
    [SerializeField] private PlayerController p2;

    [SerializeField] private GameObject p1EnemyPrefab;
    [SerializeField] private GameObject p2EnemyPrefab;

    private Vector3 _p1EnemyOffset = Vector3.zero;
    private Vector3 _p2EnemyOffset = Vector3.zero;

    private int _waveNumber;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Random.InitState(42);
    }

    public void StartNewWave()
    {
        _waveNumber++;
        StartCoroutine(TriggerWave());
    }

    private IEnumerator TriggerWave()
    {
        var numberOfEnemies = _waveNumber * 3 + 7;

        for (var i = 0; i < numberOfEnemies; i++)
        {
            var p1Direction = new Vector3(p1.MoveInput.x, p1.MoveInput.y, 0).normalized * 10;

            if (p1Direction.magnitude > 1f)
            {
                _p1EnemyOffset = Quaternion.Euler(0, 0, Random.Range(-45, 45)) * p1Direction;
            }
            else
            {
                _p1EnemyOffset = Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(10f, 15f), Random.Range(10f, 15f), 0);
            }
            
            var p2Direction = new Vector3(p1.MoveInput.x, p1.MoveInput.y, 0).normalized * 10;
            
            if (p2Direction.magnitude > 1f)
            {
                _p2EnemyOffset = Quaternion.Euler(0, 0, Random.Range(-45, 45)) * p2Direction;
            }
            else
            {
                _p2EnemyOffset = Quaternion.Euler(0, 0, Random.Range(0, 360)) * new Vector3(Random.Range(10f, 15f), Random.Range(10f, 15f), 0);
            }
            
            Instantiate(p1EnemyPrefab, p1.transform.position + _p1EnemyOffset, Quaternion.Euler(0, 0, 0));
            Instantiate(p2EnemyPrefab, p2.transform.position + _p2EnemyOffset, Quaternion.Euler(0, 0, 0));
            
            yield return new WaitForSeconds(25f / numberOfEnemies);
        }
        
        yield return null;
    }
}
