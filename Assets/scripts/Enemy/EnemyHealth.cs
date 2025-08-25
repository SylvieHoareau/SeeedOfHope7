using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 3f;
    [SerializeField] private Slider healthBar;

    private float currentHealth;
    private bool isDead = false;       // <- ajouté
    private Animator animator;         // <- ajouté

    private void Awake()
    {
        animator = GetComponent<Animator>(); // on récupère l’Animator
    }

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void Damage(float damageAmount)
    {
        if (isDead) return; // si déjà mort, pas de dégâts
        currentHealth -= damageAmount;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        animator.SetTrigger("Die"); // Lance anim de mort
        GetComponent<Collider2D>().enabled = false; // optionnel : désactive collisions
        GetComponent<Rigidbody2D>().simulated = false; // optionnel : stop physique

        Destroy(gameObject, 1f); // ou Animation Event pour caler pile la durée
    }
}
