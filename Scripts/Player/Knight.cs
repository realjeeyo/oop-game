using UnityEngine;

public class Knight : BasePlayerClass
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public CooldownUIManager cooldownUI;

    private PlayerStatsModifier statsModifier;

    [Header("Ability Settings")]
    private float abilityCooldown = 10f;
    private float cooldownTimer = 0f;
    private bool isOnCooldown = false;

    private void Start()
    {
        statsModifier = GetComponent<PlayerStatsModifier>() ?? gameObject.AddComponent<PlayerStatsModifier>();
    }

    public override void UseAbility()
    {
        if (isOnCooldown)
        {
            throw new System.InvalidOperationException("Ability is on cooldown.");
        }

        if (playerHealth.CurrentShields >= playerHealth.MaxShields)
        {
            throw new System.InvalidOperationException("Shields are already full!");
        }

        ActivateAbility();

        float finalCooldown = Mathf.Max(abilityCooldown - statsModifier.abilityCooldownReduction, 1f);
        cooldownTimer = finalCooldown;
        isOnCooldown = true;
        cooldownUI.StartCooldown(finalCooldown);
    }

    private void ActivateAbility()
    {
        playerHealth.ReplenishShield();
        Debug.Log("Knight Ability Activated: Shield Replenished!");
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                cooldownTimer = 0f;
                Debug.Log("Knight Ability is ready to use again!");
            }
        }
    }
}
