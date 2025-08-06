using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    public void Rejouer()
    {
        SceneManager.LoadScene(0); // ouvre la scène "Menu"
    }

    public void Quitter()
    {
        Application.Quit();
        Debug.Log("Le joueur quitte le jeu.");
    }
}
