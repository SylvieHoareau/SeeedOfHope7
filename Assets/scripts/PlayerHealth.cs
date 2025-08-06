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
        // Le joueur commence avec toute sa vie
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
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
            TakeDamage(20);
        }
    }

    // Cette fonction enlève de la vie au joueur quand il est touché
    public void TakeDamage(int damage)
    {
        // Si le joueur n'est pas invincible...
        if (!isInvincible)
        {
            // On joue le son de coup reçu
        if (hitSound != null)
        {
            if (audioSource != null)
            {
                Debug.Log("Joue hitSound via audioSource : " + hitSound.name);
                audioSource.PlayOneShot(hitSound);
            }
            else
            {
                Debug.LogWarning("Pas d'audioSource trouvé sur le Player !");
                AudioSource.PlayClipAtPoint(hitSound, Camera.main.transform.position, hitVolume);
            }
        }
        else
        {
            Debug.LogWarning("hitSound n’est pas assigné !");
        }

            // On enlève les points de vie
            currentHealth = Math.Max(0, currentHealth - damage);
            // On met à jour la barre de vie
            healthBar.SetHealth(currentHealth);
            // Le joueur devient invincible pendant un moment
            isInvincible = true;
            // On fait clignoter le joueur pour montrer qu'il devient invincible
            StartCoroutine(InvincibilityFlash());
            // On attend avant de pouvoir être touché à nouveau
            StartCoroutine(HandleInvincibilityDelay());
        }

        // Si la vie du joueur tombe à zéro, il meurt
        if (currentHealth <= 0)
        {
            Die();
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
    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        healthBar.SetHealth(currentHealth); // si tu as une barre de vie
    }

}
