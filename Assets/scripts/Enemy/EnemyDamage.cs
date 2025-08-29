using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    // Le script qui gère la vie du joueur
    private PlayerHealth playerHealth;
     // Les dégâts infligés au joueur
    public int damageAmount = 5;

    [Header("Paramètres des dégâts")]
    [Tooltip("Temps minimum entre chaque coup (en secondes)")]
    public float damageCooldown = 0.5f;

    private float _lastDamageTime = -9999f; // Initialisé à une valeur très basse pour permettre le premier coup

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerHealth = playerObj.GetComponent<PlayerHealth>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // 🚨 Changement de la fonction de collision !
        // On utilise OnCollisionStay2D pour vérifier continuellement la collision.

        if (collision.gameObject.CompareTag("Player") && playerHealth != null)
        {
            // Vérifie si le joueur est invincible ou si le cooldown n'est pas terminé
            if (playerHealth.isInvincible || Time.time < _lastDamageTime + damageCooldown)
            {
                // Si le joueur est invincible ou si le cooldown n'est pas terminé, on ne fait rien
                return;
            }

            // Si on est ici, le joueur n'est pas invincible et le cooldown est terminé
            Debug.Log($"Dégâts infligés au joueur : {damageAmount}");
            playerHealth.TakeDamage(damageAmount);
            _lastDamageTime = Time.time; // Met à jour le temps du dernier coup
        }
    }

    // private void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player") && playerHealth != null)
    //     {
    //         playerHealth.TakeDamage(damageAmount);
    //     }
    // }
}