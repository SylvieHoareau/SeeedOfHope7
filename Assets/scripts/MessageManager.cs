using TMPro;
using UnityEngine;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private Coroutine hideCoroutine;

    // Fonction pour afficher le message
    public void ShowMessage(string message)
    {
        // Activer le panel
        messagePanel.SetActive(true);
        // Définit le texte
        messageText.text = message;

        // Si une coruotine est dèjà en cours, on l'arrête pour éviter
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Lance la coroutine qui va cacher le panel après 10 secondes
        hideCoroutine = StartCoroutine(HidePanelAfterDelay(10f));
    }

    // Coroutine pour cacher le panel après un délai
    private IEnumerator HidePanelAfterDelay(float delay)
    {
        // Attend le nombre de secondes spécifié
        yield return new WaitForSeconds(delay);

        // Désactive le panel
        messagePanel.SetActive(false);
        // Réinitialise la coroutine
        hideCoroutine = null;
    }
}
