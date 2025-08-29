using UnityEngine;

// Ce script gère l'activation temporaire de la hitbox d'attaque
public class AttackHitboxController : MonoBehaviour
{
    // Montant des dégâts infligés par l'attaque
    public float attackDamage = 1f;
    private BoxCollider2D hitbox;

    [SerializeField] private float activeTime = 0.2f; // DurÃ©e pendant laquelle le collider est actif

    private void Awake()
    {
        // Récupère le BoxCollider2D attaché à cet object
        hitbox = GetComponent<BoxCollider2D>();
        // Désactive la hitbox au démarrage
        hitbox.enabled = false;
    }

    // Méthode à appeler pour activer la hitbox au démarrage
    public void ActivateHitbox()
    {
        // Arrête toute activation précédente
        StopAllCoroutines();
        StartCoroutine(EnableHitboxTemporarily());
    }

    // Coroutine qui active la hitbox pendant un court instant
    private System.Collections.IEnumerator EnableHitboxTemporarily()
    {
        hitbox.enabled = true;
        yield return new WaitForSeconds(activeTime);
        hitbox.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // On vérifie si on a touché un objet avec le tag "Enemy"
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // On essaie de récupérer le script EnemyHealth sur l'objet touché
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();

            if (enemyHealth != null)
            {
                // On appelle la fonction Damage du script EnemyHealth
                enemyHealth.TakeDamage(attackDamage);
            }
        }
    }
}