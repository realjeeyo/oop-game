using UnityEngine;

public class PlayerBullet : Projectile
{
    [SerializeField]
    private int bulletDamage; // Renamed to avoid conflict with BaseEnemy.damage

    public int BulletDamage
    {
        get => bulletDamage;
        set => bulletDamage = value;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            return;
        }

        if (other.CompareTag("Enemy"))
        {
            var enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(bulletDamage); // Use the renamed field
            }
        }

        base.OnTriggerEnter2D(other);
    }
}
