using UnityEngine;
using System.Collections; // Ajouté pour OnTriggerStay2D

// Ce script gère les dégâts que l'ennemi inflige au joueur
public class EnemyDamage : MonoBehaviour
{
    // Le script qui gère la vie du joueur
    private PlayerHealth playerHealth;
    // Les dégâts infligés au joueur
    public int damageAmount = 5;

    [Header("Paramètres des dégâts")]
    [Tooltip("Temps minimum entre chaque coup (en secondes)")]
    public float damageCooldown = 0.25f;

    private float _lastDamageTime = -9999f; // Initialisé à une valeur très basse pour permettre le premier coup

    void Start()
    {
        // On cherche le joueur par son tag pour s'assurer de trouver le bon objet
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            // On récupère le composant PlayerHealth du joueur
            playerHealth = playerObj.GetComponent<PlayerHealth>();
            // Log pour confirmer que le script PlayerHealth est bien trouvé
            Debug.Log("PlayerHealth script trouvé sur l'objet joueur.");
        }
        else
        {
            Debug.LogError("Objet joueur non trouvé ! Assurez-vous que le joueur a bien le tag 'Player'.");
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Log pour vérifier que l'ennemi est bien en collision avec quelque chose
        Debug.Log("Collision détectée par l'ennemi avec : " + collision.gameObject.name);

        // On utilise OnCollisionStay2D pour vérifier continuellement la collision.
        if (collision.gameObject.CompareTag("Player") && playerHealth != null)
        {
            // Log pour confirmer que l'objet en collision est bien le joueur
            Debug.Log("L'ennemi est en contact avec le joueur !");

            // Vérifie si le joueur n'est pas déjà invincible
            if (!playerHealth.isInvincible && Time.time >= _lastDamageTime + damageCooldown)
            {
                // Affiche les dégâts et la vie restante
                Debug.Log($"Dégâts infligés au joueur : {damageAmount}. Vie restante du joueur avant les dégâts : {playerHealth.currentHealth}");

                playerHealth.TakeDamage(damageAmount);
                _lastDamageTime = Time.time; // Met à jour le temps du dernier coup
            }
            else
            {
                 // Log pour comprendre pourquoi les dégâts ne sont pas infligés
                if (playerHealth.isInvincible)
                {
                    Debug.Log("Le joueur est invincible, les dégâts sont ignorés.");
                }
                else
                {
                    Debug.Log("Le cooldown n'est pas encore terminé. Temps restant : " + (_lastDamageTime + damageCooldown - Time.time));
                }
            }
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