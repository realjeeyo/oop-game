using UnityEngine;

public class PlayerStatsModifier : MonoBehaviour
{
    [Header("Base Multipliers")]
    public float baseDamageMultiplier = 1f;     // Base permanent multiplier
    public float baseMoveSpeedMultiplier = 1f; // Base permanent movement multiplier
    public float baseAttackSpeedMultiplier = 1f; // Base permanent attack speed multiplier
    public float abilityCooldownReduction = 0f; // Default cooldown reduction in seconds

    [Header("Temporary Buffs")]
    private float tempDamageMultiplier = 1f;   // Temporary damage multiplier
    private float tempMoveSpeedMultiplier = 1f; // Temporary movement speed multiplier
    private float tempAttackSpeedMultiplier = 1f; // Temporary attack speed multiplier

    public float damageMultiplier => baseDamageMultiplier * tempDamageMultiplier;
    public float moveSpeedMultiplier => baseMoveSpeedMultiplier * tempMoveSpeedMultiplier;
    public float attackSpeedMultiplier => baseAttackSpeedMultiplier * tempAttackSpeedMultiplier;

    public void ApplyPermanentDamageMultiplier(float multiplier)
    {
        baseDamageMultiplier *= multiplier;
        Debug.Log("Permanent Damage multiplier applied: " + baseDamageMultiplier);
    }

    public void ApplyPermanentMoveSpeedBoost(float multiplier)
    {
        baseMoveSpeedMultiplier *= multiplier;
        Debug.Log("Permanent Move Speed multiplier applied: " + baseMoveSpeedMultiplier);
    }

    public void ApplyPermanentAttackSpeedBoost(float multiplier)
    {
        baseAttackSpeedMultiplier *= multiplier;
        Debug.Log("Permanent Attack Speed multiplier applied: " + baseAttackSpeedMultiplier);
    }

    public void ApplyTemporaryDamageMultiplier(float multiplier)
    {
        tempDamageMultiplier *= multiplier;
        Debug.Log("Temporary Damage multiplier applied: " + tempDamageMultiplier);
    }

    public void ApplyTemporaryMoveSpeedBoost(float multiplier)
    {
        tempMoveSpeedMultiplier *= multiplier;
        Debug.Log("Temporary Move Speed multiplier applied: " + tempMoveSpeedMultiplier);
    }

    public void ApplyTemporaryAttackSpeedBoost(float multiplier)
    {
        tempAttackSpeedMultiplier *= multiplier;
        Debug.Log("Temporary Attack Speed multiplier applied: " + tempAttackSpeedMultiplier);
    }

    public void ResetTemporaryModifiers()
    {
        tempDamageMultiplier = 1f;
        tempMoveSpeedMultiplier = 1f;
        tempAttackSpeedMultiplier = 1f;
        Debug.Log("Temporary modifiers reset.");
    }
}
