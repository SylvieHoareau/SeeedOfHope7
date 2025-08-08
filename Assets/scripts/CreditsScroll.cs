using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScroll : MonoBehaviour
{
    // La vitesse à laquelle le texte défile.
    public float scrollSpeed = 50f; 

    // Temps d'attente avant de retourner à la scène du menu.
    public float timeBeforeReturningToMenu = 5f; 
    
    // Le RectTransform du panneau qui contient tout le texte.
    private RectTransform rectTransform;

    void Start()
    {
        // Récupère le composant RectTransform du GameObject sur lequel ce script est attaché.
        rectTransform = GetComponent<RectTransform>();

        // Lance le défilement des crédits.
        StartScrolling();
    }

    void Update()
    {
        // Déplace le RectTransform vers le haut à chaque frame.
        rectTransform.Translate(Vector3.up * scrollSpeed * Time.deltaTime);
    }
    
    // Fonction qui attend la fin du défilement et charge la scène du menu.
    void StartScrolling()
    {
        // Calcule le temps nécessaire pour que tout le texte défile.
        // C'est le temps total du défilement + le temps d'attente.
        float totalScrollTime = rectTransform.rect.height / scrollSpeed;
        
        // Appelle la fonction pour retourner au menu après le défilement.
        Invoke("ReturnToMenu", totalScrollTime + timeBeforeReturningToMenu);
    }

    void ReturnToMenu()
    {
        // Charge la scène du menu. Assure-toi que "Menu" est le nom correct de ta scène de menu.
        SceneManager.LoadScene("Menu"); 
    }
}