using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Cette fonction sera appelée par le bouton pour charger une scène
    public void LoadNextScene()
    {
        // On charge la scène suivante dans l'ordre de la "Build Settings"
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}