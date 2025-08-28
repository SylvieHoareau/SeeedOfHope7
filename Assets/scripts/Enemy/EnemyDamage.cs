using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    // Le script qui gère la vie du joueur
    private PlayerHealth playerHealth;
     // Les dégâts infligés au joueur
    public int damageAmount = 10;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
            playerHealth = playerObj.GetComponent<PlayerHealth>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && playerHealth != null)
        {
            playerHealth.TakeDamage(damageAmount);
        }
    }
}