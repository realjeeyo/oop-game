using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float baseMoveSpeed = 2f;       // Default movement speed
    public Camera mainCamera;             // Reference to the main camera
    private Rigidbody2D rb2d;             // Rigidbody2D for movement
    private Vector2 movement;             // 2D movement vector
    private Animator animator;            // Animator for controlling animations

    private BasePlayerClass playerClass;  // Reference to the player's class/ability system
    private PlayerStatsModifier statsModifier; // Reference to the stats modifier

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Get the player's class
        playerClass = GetComponent<BasePlayerClass>();
        if (playerClass == null)
        {
            Debug.LogWarning("No player class assigned.");
        }

        // Get the stats modifier
        statsModifier = GetComponent<PlayerStatsModifier>();
        if (statsModifier == null)
        {
            statsModifier = gameObject.AddComponent<PlayerStatsModifier>();
        }
    }

    void Update()
    {
        // Get movement input (WASD or Arrow keys)
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);
        // Calculate the current move speed from the base speed and any modifiers
        float currentMoveSpeed = baseMoveSpeed * statsModifier.moveSpeedMultiplier;

        // Create movement vector
        movement = new Vector2(moveX, moveY).normalized * currentMoveSpeed;

        // Flip player sprite based on mouse position
        FlipPlayerSprite();

        // Use the player's ability on right-click
        if (Input.GetMouseButtonDown(1)) // Right mouse button
        {
            playerClass?.UseAbility();
        }
    }

    void FixedUpdate()
    {
        // Apply movement to the Rigidbody2D
        rb2d.velocity = movement;
    }

    void FlipPlayerSprite()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        // Flip the player sprite based on the mouse's X position relative to the player
        if (mousePosition.x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1); // Face left
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1); // Face right
        }
    }
}
