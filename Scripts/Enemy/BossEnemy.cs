using UnityEngine;
using System.Collections;

public class BossEnemy : BaseEnemy
{
    [Header("Prefabs")]
    public GameObject minionPrefab;
    public GameObject projectilePrefab;
    public GameObject aoePrefab;

    [Header("Action Timers")]
    public float actionDuration = 5f;
    public float summonInterval = 2f;
    public float projectileInterval = 1f;
    public float aoeInterval = 3f;

    [Header("Charge Settings")]
    public float chargeSpeed = 8f;
    public float chargeDuration = 2f;
    private Vector2 chargeDirection;

    [Header("Coin Drop Settings")]
    public GameObject coinPrefab;
    public int coinDropCount = 5;

    private float actionTimer = 0f;
    private float cooldownTimer = 0f;
    private float chargeTimer = 0f;

    private BossState currentState;
    private Vector2 facingDirection = Vector2.right;

    private enum BossState
    {
        Idle,
        Summoning,
        ShootingProjectiles,
        AoEAttack,
        Charging
    }

    protected override void Start()
    {
        base.Start();
        currentState = BossState.Idle;
        PickRandomAction();
    }

    protected override void Update()
    {
        base.Update();

        if (isDead) return;

        actionTimer += Time.deltaTime;
        switch (currentState)
        {
            case BossState.Summoning:
                HandleSummoning();
                break;
            case BossState.ShootingProjectiles:
                HandleShootingProjectiles();
                break;
            case BossState.AoEAttack:
                HandleAoEAttack();
                break;
            case BossState.Charging:
                HandleCharging();
                break;
        }

        if (actionTimer >= actionDuration)
        {
            actionTimer = 0f;
            PickRandomAction();
        }

        HandleFlipping();
    }

    private void PickRandomAction()
    {
        currentState = (BossState)Random.Range(0, System.Enum.GetValues(typeof(BossState)).Length);
        cooldownTimer = 0f;

        if (currentState == BossState.Charging)
        {
            StartCharge();
        }
    }

    private void HandleSummoning()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= summonInterval)
        {
            SummonMinions();
            cooldownTimer = 0f;
        }
    }

    private void SummonMinions()
    {
        if (minionPrefab == null) return;

        for (int i = 0; i < 3; i++)
        {
            Vector2 spawnOffset = UnityEngine.Random.insideUnitCircle * 2f;
            Vector3 spawnPosition = transform.position + (Vector3)spawnOffset;

            GameObject minion = Instantiate(minionPrefab, spawnPosition, Quaternion.identity);
            minion.SetActive(true);
        }
    }

    private void HandleShootingProjectiles()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= projectileInterval)
        {
            FireProjectiles();
            cooldownTimer = 0f;
        }
    }

    private void FireProjectiles()
    {
        if (projectilePrefab == null || player == null) return;

        int numberOfProjectiles = 5;
        float spreadAngle = 45f;
        float projectileSpeed = 5f;

        Vector2 playerDirection = (player.position - transform.position).normalized;
        float baseAngle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg;

        for (int i = 0; i < numberOfProjectiles; i++)
        {
            float angle = baseAngle - spreadAngle / 2 + (spreadAngle / (numberOfProjectiles - 1)) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            projectile.SetActive(true);

            Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * projectileSpeed;
            }
        }
    }

    private void HandleAoEAttack()
    {
        cooldownTimer += Time.deltaTime;
        if (cooldownTimer >= aoeInterval)
        {
            TriggerAoE();
            cooldownTimer = 0f;
        }
    }

    private void TriggerAoE()
    {
        if (aoePrefab == null) return;

        GameObject aoe = Instantiate(aoePrefab, transform.position, Quaternion.identity);
        aoe.SetActive(true);

        int numberOfFireballs = 8;
        float spreadAngle = 360f;
        float fireballSpeed = 6f;

        for (int i = 0; i < numberOfFireballs; i++)
        {
            float angle = (spreadAngle / numberOfFireballs) * i;
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            GameObject fireball = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            fireball.SetActive(true);

            Rigidbody2D rb = fireball.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = direction * fireballSpeed;
            }
        }
    }

    private void HandleCharging()
    {
        transform.Translate(chargeDirection.normalized * chargeSpeed * Time.deltaTime);
        chargeTimer += Time.deltaTime;

        if (chargeTimer >= chargeDuration)
        {
            chargeTimer = 0f;
            currentState = BossState.Idle;
        }
    }

    private void StartCharge()
    {
        if (player != null)
        {
            chargeDirection = (player.position - transform.position).normalized;
        }
    }

    private void HandleFlipping()
    {
        Vector2 direction = chargeDirection.sqrMagnitude > 0 ? chargeDirection : (player != null ? (player.position - transform.position).normalized : Vector2.right);
        SetSpriteFlip(direction.x < 0);
    }

    private void SetSpriteFlip(bool isFlipped)
    {
        Vector3 fixedScale = new Vector3(3.7577f, 3.7577f, 3.7577f);
        fixedScale.x *= isFlipped ? -1 : 1;
        transform.localScale = fixedScale;
    }

    

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;
        DropCoins(coinDropCount);

        // Trigger difficulty increase globally
        EnemySpawner.IncreaseGlobalDifficulty();

        // Explicitly notify the spawner
        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(this);
        }
        else
        {
            Debug.LogWarning("BossEnemy: EnemySpawner reference is null, cannot remove boss from active enemy list.");
        }

        Debug.Log("Boss defeated. Triggering OnDeath and increasing global difficulty.");
        Destroy(gameObject);
    }
    



    private void DropCoins(int count)
    {
        if (coinPrefab == null) return;

        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * 0.5f;
            Vector3 dropPosition = transform.position + (Vector3)randomOffset;

            GameObject coin = Instantiate(coinPrefab, dropPosition, Quaternion.identity);
            coin.SetActive(true);
        }
    }

    public void ScaleStrength(float multiplier)
    {
        health = Mathf.RoundToInt(health * multiplier);
        moveSpeed *= multiplier;

        Debug.Log($"Boss stats scaled by {multiplier}: Health = {health}, MoveSpeed = {moveSpeed}");
    }


    public override void Attack()
    {
        // Randomized attacks are handled in the Update loop.
    }
}
