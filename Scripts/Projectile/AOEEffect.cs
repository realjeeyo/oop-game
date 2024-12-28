using UnityEngine;

public class AOEEffect : MonoBehaviour
{
    public float damage = 1f;          // Damage dealt to the player
    public float moveSpeed = 6f;       // Speed of the AOE movement
    public Vector2 targetPosition;     // The target position to move toward
    public float duration = 5f;        // Duration before the AOE disappears

    private void Start()
    {
        // Optionally, play a visual effect or sound here
        Invoke("DestroySelf", duration); // Destroy the object after the duration
    }

    private void Update()
    {
        // Move the AOE effect toward the target position
        Vector2 currentPosition = transform.position;
        Vector2 direction = (targetPosition - currentPosition).normalized;
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // Check if the AOE has reached close to the target position
        if (Vector2.Distance(currentPosition, targetPosition) < 0.1f)
        {
            DestroySelf();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Damage the player only
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage((int)damage);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject); // Destroy the AOE object
    }
}
