using UnityEngine;

namespace Menu
{
    public class LoseScore : MonoBehaviour
    {
        [HideInInspector] public Vector3 scorePosition;
        private Vector3 _explosionPosition;
        private Quaternion _desiredRotation;
        private float _amplitude;
        private float _amplitudeVariation;
        
        private void Start()
        {
            _explosionPosition = Quaternion.Euler(0f, 0f, Random.Range(0f,360f)) * new Vector3(Random.Range(0.25f,0.75f),Random.Range(0.25f,0.75f), transform.position.z);
            _desiredRotation = Quaternion.Euler(0f,0f, Random.Range(-90f,90f));
            _amplitude = 0f;
            _amplitudeVariation = 0.001f;
        }

        private void FixedUpdate()
        {
            if (_amplitude < 1f){
                transform.position = Vector3.Lerp(scorePosition, scorePosition + _explosionPosition, _amplitude);
                transform.rotation = Quaternion.Slerp(transform.rotation, _desiredRotation, _amplitude);
                _amplitude += 0.04f;
                _amplitude += _amplitudeVariation;
                _amplitudeVariation -= 0.00005f;
                _amplitudeVariation = Mathf.Clamp(_amplitudeVariation, 0f, 1f);
                _amplitude = Mathf.Clamp(_amplitude, 0f, 1f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

