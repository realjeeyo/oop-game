using UnityEngine;
using UnityEngine.UI;

public class CooldownUIManager : MonoBehaviour
{
    public Image cooldownImage;      // Cooldown progress image
    public Sprite phoenixAbilityIcon; // Icon for Phoenix ability
    public Sprite jettAbilityIcon;    // Icon for Jett ability

    private float cooldownDuration;   // Total cooldown time
    private float cooldownRemaining; // Remaining cooldown time

    private bool isCooldownActive = false;

    // Set the ability icon dynamically based on player class
    public void SetAbilityIcon(string playerClass)
    {
        if (cooldownImage == null) return;

        switch (playerClass)
        {
            case "Phoenix":
                cooldownImage.sprite = phoenixAbilityIcon;
                break;
            case "Jett":
                cooldownImage.sprite = jettAbilityIcon;
                break;
        }

        cooldownImage.fillAmount = 1f; // Reset fill
        cooldownImage.gameObject.SetActive(true); // Ensure the image is active
    }

    // Start the cooldown
    public void StartCooldown(float duration)
    {
        cooldownDuration = duration;
        cooldownRemaining = duration;
        isCooldownActive = true;

        cooldownImage.fillAmount = 0f; // Start empty
    }

    private void Update()
    {
        if (isCooldownActive)
        {
            cooldownRemaining -= Time.deltaTime;

            if (cooldownRemaining <= 0)
            {
                cooldownRemaining = 0;
                isCooldownActive = false;

                cooldownImage.fillAmount = 1f; // Full after cooldown finishes
                return;
            }

            // Update the image fill amount (reverse fill)
            cooldownImage.fillAmount = 1f - (cooldownRemaining / cooldownDuration);
        }
    }
}
