using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    // Ajout d'une référence au premier bouton que le menu doit sélectionner
    [SerializeField] private GameObject firstSelectedButton;

    private bool isPaused = false;

    // Propriété publique pour que PauseInput y accède
    public bool IsPaused => isPaused;

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

         // Assure que l'EventSystem a un bouton sélectionné
        // C'est la ligne la plus importante pour la manette
        if (firstSelectedButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstSelectedButton);
        }
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        // Désélectionne le bouton pour éviter une interaction involontaire après la reprise
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Home()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
