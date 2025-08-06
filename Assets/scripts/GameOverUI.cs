using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    public AudioClip gameOverSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (gameOverSound != null)
        {
            if (audioSource != null)
            {
                audioSource.PlayOneShot(gameOverSound);
            }
            else
            {
                AudioSource.PlayClipAtPoint(gameOverSound, Camera.main.transform.position, 0.5f); // Volume ajusté à 0.5
                Debug.LogWarning("Pas d'AudioSource sur GameOverUI, son joué avec PlayClipAtPoint.");
            }
        }
        else
        {
            Debug.LogWarning("Aucun son Game Over assigné !");
        }
    }

    public void RetryGame()
    {
        string previousLevel = PlayerPrefs.GetString("LastPlayedLevel", "");

        if (!string.IsNullOrEmpty(previousLevel))
        {
            SceneManager.LoadScene(previousLevel);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitter le jeu");
    }
}
