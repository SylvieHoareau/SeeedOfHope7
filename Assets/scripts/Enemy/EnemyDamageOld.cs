using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamageOld : MonoBehaviour
{
    public PlayerHealth playerHealth;

    // Points de vie de l'ennemi
    [Header("Santé")]
    [SerializeField] private int maxHealth = 10;
    private int currentHealth;

    [Header("UI")]
    // Référence à la barre de vie (le slider)
    [SerializeField] private Slider healthBar;

    [Header("Effets et Sons")]
    // Effet de particules pour les dégâts
    [SerializeField] private GameObject hitEffect;
    // Son lorsque l'ennemi subit des dégâts
    [SerializeField] private AudioClip hitSound;
    private AudioSource audioSource;

    void Awake()
    {
        currentHealth = maxHealth;
        // On récupère le composant AudioSource s'il existe
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        // On s'assure que la barre de vie est bien assignée
        if (healthBar == null)
        {
            Debug.LogError("La barre de vie n'est pas assignée dans l'inspecteur !");
        }
        else
        {
            // On initialise la barre de vie avec la santé maximale
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    /// <summary>
    /// Fonction publique pour infliger des dégâts à l'ennemi
    /// Elle est appelée par d'autres scripts (comme celui du joueur)
    /// </summary>
    /// <param name="damageAmount">La quantité de dégâts à infliger.</param>
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"L'ennemi {gameObject.name} a {currentHealth} points de vie restants.");

        // Joue un effet visuel ou sonore de dégâts
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
        }
        if (audioSource != null && hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }

        // Si la santé est égale ou inférieur à zéro, l'ennemi meurt
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    private void Die()
    {
        Debug.Log($"L'ennemi {gameObject.name} est mort.");
        // Détruire l'objet ennemi
        Destroy(gameObject);
        // Vous pouvez ajouter d'autres actions ici, comme une animation de mort ou un butin
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(20);

        }
    }
}
