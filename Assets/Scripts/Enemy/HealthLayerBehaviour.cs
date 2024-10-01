using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
     public class HealthLayerBehaviour : MonoBehaviour
    {
        private SpriteRenderer _sprite;
        private bool _isPlayer1;

        private RectTransform _universe;
        private Vector3 _universePosition;

        private float _score;

        private void Awake()
        {
            _sprite = GetComponent<SpriteRenderer>();
        }

        private void FixedUpdate()
        {
            _universePosition = _universe.TransformPoint(_universe.pivot);
        }

        public void InitializeHealthLayer(Color color, bool isPlayer1)
        {
            _isPlayer1 = isPlayer1;
            _sprite.color = color;

            _universe = _isPlayer1 
                ? GameObject.FindGameObjectWithTag("P1 Universe").GetComponent<RectTransform>()
                : GameObject.FindGameObjectWithTag("P2 Universe").GetComponent<RectTransform>();;
        }

        public void SetParameters(float score)
        {
            _score = score;
            transform.localScale = new Vector3(0.11f + _score * 0.03f, 0.075f, 1f);
        }

        public void StartConversionHealthToScore(Vector3 impactPoint)
        {
            StartCoroutine(ConvertHealthToScore(impactPoint));
        }

        private IEnumerator ConvertHealthToScore(Vector3 impactPoint)
        {
            var explosionPoint = 
                Quaternion.Euler(0f, Random.Range(-15f,15f), 0f) 
                * new Vector3(
                    transform.position.x + impactPoint.x * Random.Range(0.5f,0.75f), 
                    transform.position.y + impactPoint.y * Random.Range(0.5f,0.75f), 
                    transform.position.z
                );
            var desiredRotation = Quaternion.Euler(0f, 0f, Random.Range(-90f, 90f));
            var range = 0f;
            var rangeVariation = 0.001f;

            while ((transform.position - explosionPoint).magnitude > 0.003f)
            {
                transform.position = Vector3.Lerp(transform.position, explosionPoint, range);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, range);
                range += 0.015f + rangeVariation;
                rangeVariation -= 0.00005f;
                rangeVariation = Mathf.Clamp(rangeVariation, 0f, 1f);

                yield return new WaitForFixedUpdate();
            }
            
            desiredRotation = Quaternion.Euler(0f, 0f, Random.Range(-90f, 90f));
            range = 0f;
            rangeVariation = 0.001f;

            while ((transform.position - _universePosition).magnitude > 2f)
            {
                transform.position = Vector3.Lerp(transform.position, _universePosition, range);
                transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, range);
                range += 0.0001f +rangeVariation;
                rangeVariation += 0.00005f;
                
                yield return new WaitForFixedUpdate();
            }
            
            Destroy(gameObject);
            
            yield return null;
        }
    }   
}
