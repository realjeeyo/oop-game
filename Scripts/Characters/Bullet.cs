using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {
        // Example: Destroy the bullet on impact
        Destroy(gameObject);
    }
}
