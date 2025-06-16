using System;
using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class WaveManager : MonoBehaviour
    {
        public static WaveManager Instance { get; private set; }

        [SerializeField] private PlayerController p1;
        [SerializeField] private PlayerController p2;

        [SerializeField] private GameObject p1EnemyPrefab;
        [SerializeField] private GameObject p2EnemyPrefab;

        [SerializeField] private Camera mainCamera;

        private int _waveNumber;
        private const float Extra = 2f;

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
                var p1EnemySpawnPos = GetSpawnPositionOutsideCamera();
                var p2EnemySpawnPos = GetSpawnPositionOutsideCamera();

                Instantiate(p1EnemyPrefab, p1EnemySpawnPos, Quaternion.identity);
                Instantiate(p2EnemyPrefab, p2EnemySpawnPos, Quaternion.identity);

                yield return new WaitForSeconds(25f / numberOfEnemies);
            }

            yield return null;
        }
        
        private Vector3 GetSpawnPositionOutsideCamera()
        {
            var camHeight = mainCamera.orthographicSize * 2f;
            var camWidth = camHeight * mainCamera.aspect;

            Vector2 camCenter = mainCamera.transform.position;
            var halfWidth = camWidth / 2f;
            var halfHeight = camHeight / 2f;

            var side = Random.Range(0, 4);
            
            return side switch
            {
                0 => new Vector2(
                    Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth),
                    camCenter.y + halfHeight + Extra
                ),
                1 => new Vector2(
                    Random.Range(camCenter.x - halfWidth, camCenter.x + halfWidth),
                    camCenter.y - halfHeight - Extra
                ),
                2 => new Vector2(
                    camCenter.x - halfWidth - Extra,
                    Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight)
                ),
                3 => new Vector2(
                    camCenter.x + halfWidth + Extra,
                    Random.Range(camCenter.y - halfHeight, camCenter.y + halfHeight)
                ),
                _ => camCenter
            };

        }
    }
}
