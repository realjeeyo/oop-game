using UnityEngine;

public class LockPosition : MonoBehaviour
{
    private Vector3 initialPosition;

    void Start()
    {
        // Store the initial position
        initialPosition = transform.position;
    }

    void LateUpdate()
    {
        // Lock the position to the initial position
        transform.position = initialPosition;
    }
}
