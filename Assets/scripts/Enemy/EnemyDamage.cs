using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private PlayerHealth playerHealth;

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
            playerHealth.TakeDamage(10);
        }
    }
}