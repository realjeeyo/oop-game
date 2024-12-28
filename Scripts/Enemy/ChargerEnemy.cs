using UnityEngine;

public class ChargerEnemy : BaseEnemy
{
    public float chargeSpeed = 8f;       // Speed of the charge
    public float chargeDelay = 1f;      // Delay before charging
    public float chargeDuration = 2f;  // Duration of the charge
    public float cooldownDuration = 1.5f; // Cooldown after charging

    private Vector2 moveDirection;      // Direction to charge
    private float chargeTimer = 0f;     // Timer for charging duration
    private float cooldownTimer = 0f;  // Timer for cooldown
    private EnemyState currentState = EnemyState.Idle; // Current enemy state

    private enum EnemyState
    {
        Idle,       // Waiting before charge
        Charging,   // Actively charging
        Cooldown    // Resting after charge
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        switch (currentState)
        {
            case EnemyState.Idle:
                HandleIdleState();
                break;

            case EnemyState.Charging:
                HandleChargingState();
                break;

            case EnemyState.Cooldown:
                HandleCooldownState();
                break;
        }
    }

    private void HandleIdleState()
    {
        // Wait for the charge delay, then transition to charging
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= chargeDelay)
        {
            cooldownTimer = 0f;
            StartCharge();
        }
    }

    private void HandleChargingState()
    {
        // Move in the determined direction
        transform.Translate(moveDirection.normalized * chargeSpeed * Time.deltaTime);

        // Stop charging after the charge duration
        chargeTimer += Time.deltaTime;
        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            StopCharge();
        }
    }

    private void HandleCooldownState()
    {
        // Wait for the cooldown to finish, then return to idle
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= cooldownDuration)
        {
            cooldownTimer = 0f;
            currentState = EnemyState.Idle;
        }
    }

    private void StartCharge()
    {
        if (player != null)
        {
            // Calculate direction toward player
            moveDirection = (player.position - transform.position).normalized;
        }
        currentState = EnemyState.Charging;
    }

    private void StopCharge()
    {
        currentState = EnemyState.Cooldown;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Deal damage to the player on collision
            other.GetComponent<PlayerHealth>().TakeDamage(damage);

            // Optionally stop charging on collision
            StopCharge();
        }
    }

    protected override void Die()
    {
        base.Die();
        // Add any specific death logic here (e.g., explosion, effects)
    }

    public override void Attack()
    {
        // Charger-specific attack behavior can be implemented here if needed.
        // Since charging is handled in Update, this method can remain empty for now.
    }
}
