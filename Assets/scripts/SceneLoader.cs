using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Cette fonction sera appelée par le bouton pour charger la scène suivante
    public void LoadNextScene()
    {
        // Récupère l'index de la scène active
        int currentIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
        int nextIndex = currentIndex + 1;

        // Vérifie que la scène suivante existe dans les Build Settings
        if (nextIndex < UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.LogWarning($"SceneLoader: pas de scène suivante dans Build Settings (index courant = {currentIndex}).");
            // Optionnel : revenir à la première scène
            // UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}