using UnityEngine;

public class Thief : BasePlayerClass
{
    public PlayerStatsModifier statsModifier;
    public CooldownUIManager cooldownUI; // Reference to CooldownUIManager

    private float abilityDuration = 5f; // How long the ability remains active
    private float attackSpeedBoost = 1.5f;
    private float moveSpeedBoost = 1.3f;
    private float cooldownTime = 15f; // Cooldown after ability ends

    private float cooldownTimer = 0f; // Time until the ability can be used again
    private float abilityTimer = 0f;  // Tracks how long the ability remains active
    private bool isAbilityActive = false;

    void Update()
    {
        // Reduce cooldown timer if active
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Handle ability duration timer
        if (isAbilityActive)
        {
            abilityTimer -= Time.deltaTime;
            if (abilityTimer <= 0f)
            {
                ResetAbility(); // End ability once duration ends
            }
        }
    }

    public override void UseAbility()
    {
        if (cooldownTimer <= 0f) // Only activate if not on cooldown
        {
            ActivateAbility();

            // Start cooldown timer (cooldown starts when the ability is activated)
            cooldownTimer = cooldownTime;

            // Trigger the cooldown UI (total cooldown time)
            cooldownUI.StartCooldown(cooldownTime);
        }
    }

    private void ActivateAbility()
    {
        isAbilityActive = true;
        abilityTimer = abilityDuration; // Set the ability duration timer

        if (statsModifier != null)
        {
            statsModifier.ApplyTemporaryAttackSpeedBoost(attackSpeedBoost);
            statsModifier.ApplyTemporaryMoveSpeedBoost(moveSpeedBoost);
        }

        Debug.Log("Thief ability activated!");
    }

    private void ResetAbility()
    {
        isAbilityActive = false; // End the ability

        if (statsModifier != null)
        {
            statsModifier.ResetTemporaryModifiers(); // Remove temporary stat boosts
        }

        Debug.Log("Thief ability reset.");
    }
}
