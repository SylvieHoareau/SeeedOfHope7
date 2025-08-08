using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryUI : MonoBehaviour
{
    public float delayBeforeCredits = 10f; // Temps avant d'aller aux crédits
    private bool joueurARejoue = false;

    void Start()
    {
        // Lance le compte à rebours vers les crédits
        Invoke(nameof(LancerCreditsSiInactif), delayBeforeCredits);
    }

    public void Rejouer()
    {
        SceneManager.LoadScene(0); // ouvre la scène "Menu"
    }

    public void Quitter()
    {
        Application.Quit();
        Debug.Log("Le joueur quitte le jeu.");
    }
    
     private void LancerCreditsSiInactif()
    {
        if (!joueurARejoue)
        {
            // Charge la scène des crédits
            SceneManager.LoadScene("Credits"); 
        }
    }
}
