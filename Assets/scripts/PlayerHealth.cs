using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

// Ce script gère la vie du joueur dans le jeu
public class PlayerHealth : MonoBehaviour
{
    

    // Indique si le joueur est temporairement invincible après avoir été touché
    public bool isInvincible = false;
    // Permet d'afficher le joueur à l'écran (utile pour faire clignoter le joueur quand il est touché)
    public SpriteRenderer graphics;
    // Vie maximale du joueur
    public int maxHealth = 100;
    // Vie actuelle du joueur
    public int currentHealth;
    // Temps pendant lequel le joueur reste invincible après avoir été touché
    public float InvincibilityTimeAfterHit = 3f;
    // Barre de vie affiché à l'écran
    public HealthBar healthBar;

    // Son joué quand le joueur est touché
    public AudioClip hitSound; // glisser le son dans l'inspecteur
    public AudioClip healSound; // glisser le son dans l'inspecteur


    // Permet de jouer le son
    private AudioSource audioSource;

    // Interface affichée quand le joueur perd (Game Over)
    public GameObject gameOverUI;
    // Temps entre chaque clignotement du joueur quand il est invincible
    public float InvincibilityFlashDelay = 0.2f;
    private float hitVolume = 0.5f;

    // Cette fonction est appelé au début du jeu
    public void Start()
    {
        currentHealth = maxHealth;
        // Le joueur commence avec toute sa vie
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            Debug.LogWarning("HealthBar n'est pas assigné sur PlayerHealth !");
        }
        // On cache l'interface de Game Over au début
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        // On sauvegarde le niveau actuel
        PlayerPrefs.SetString("LastPlayedLevel", SceneManager.GetActiveScene().name);

        // On prépare le son à jouer
        audioSource = GetComponent<AudioSource>();
    }

    // Cette fonction est appelée à chaque image du jeu
    public void Update()
    {
        // Si on appuie sur la touche H, le joueur perd 20 points de vie (pour tester)
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log("Touche H pressée. Dégâts de test envoyés.");
            TakeDamage(20);
        }
    }

    // Cette fonction enlève de la vie au joueur quand il est touché
   public void TakeDamage(int damageAmount)
    {
        // Log au début de la fonction pour vérifier si elle est appelée
        Debug.Log($"La fonction TakeDamage est appelée ! Dégâts : {damageAmount}");

        // On n'inflige pas de dégâts si le joueur est invincible
        if (isInvincible)
        {
            // Log si le joueur est invincible
            Debug.Log("Le joueur est invincible, les dégâts sont ignorés.");
            return;
        }

        // On enlève le nombre de points de vie défini par le script de l'ennemi
        currentHealth -= damageAmount;

        // On affiche les dégâts infligés et la vie restante
        Debug.Log($"Le joueur a perdu {damageAmount} point(s) de vie. Vie restante : {currentHealth}");

        // Met à jour la barre de vie du joueur
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // Si la vie tombe à zéro, on lance la fonction de mort
        if (currentHealth <= 0)
        {
            Die();
        }

        // Active l'invincibilité temporaire et les effets visuels
        isInvincible = true;
        StartCoroutine(InvincibilityFlash());
        StartCoroutine(HandleInvincibilityDelay());
        
        // Joue un son de dégâts si disponible
        if (hitSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hitSound, hitVolume);
        }
    }

    // Cette fonction est appelée quand le joueur n'a plus de vie
    void Die()
    {
        Debug.Log("Le joueur est mort !");

        // ✅ Sauvegarde du nom de la scène actuelle dans PlayerPrefs
        string currentSceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString("LastPlayedLevel", currentSceneName);

        // On affiche à l'interface Game Over
        if (gameOverUI != null)
        {
            Debug.Log("UI trouvée ! Activation...");
            gameOverUI.SetActive(true);
        }
        else
        {
            Debug.LogWarning("gameOverUI n'est pas assigné !");
        }

        // ✅ Chargement de la scène GameOver après l'affichage de l'UI
        SceneManager.LoadScene("GameOver");
    }

    // Cette fonction fait clignoter le joueur quand il est invincible
    public IEnumerator InvincibilityFlash()
    {
        while (isInvincible)
        {
            graphics.color = new Color(1f, 1f, 1f, 0f); // invisible
            yield return new WaitForSeconds(InvincibilityFlashDelay);
            graphics.color = new Color(1f, 1f, 1f, 1f); // visible
            yield return new WaitForSeconds(InvincibilityFlashDelay);
        }
    }

    // Cette fonction gère le temps d'invincibilité après avoir été touché
    public IEnumerator HandleInvincibilityDelay()
    {
        yield return new WaitForSeconds(InvincibilityTimeAfterHit);
        isInvincible = false;
    }

    // Cette fonction gère le soin du joueur pendant le niveau 4
    // public void Heal(int amount)
    // {
    //     currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    //     healthBar.SetHealth(currentHealth); // si tu as une barre de vie
    // }
    
    // Ajoute de la vie au joueur, sans dépasser la vie maximale
    public void AddHealth(int amount)
    {
        if (amount <= 0)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);

        // Joue un son de soin si disponible
        if (healSound != null && audioSource != null)
            audioSource.PlayOneShot(healSound);

        Debug.Log("Vie ajoutée : " + amount + " | Vie actuelle : " + currentHealth);
    }


}
