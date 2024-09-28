using UnityEngine;
using UnityEngine.InputSystem;

public class PistolRotation : MonoBehaviour
{
    public float rotationSpeed = 100f;
    [HideInInspector] public bool isPlayerConnected = false;

    private void Update()
    {
        if (!isPlayerConnected)
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}