using UnityEngine;

// Ce script gère l'effet de knockback sur les ennemis touchés par la 
public class AttackHitbox : MonoBehaviour
{
    // Force du knockback
    public float knockbackForce = 5f;

    // Détecte la collision avec un ennemi
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Calcul la direction du knockback
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;
                // Applique la force de knockback
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            }
        }
    }
}