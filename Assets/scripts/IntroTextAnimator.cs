using System.Collections;
using UnityEngine;
using TMPro;

public class IntroTextAnimator : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    [TextArea(3, 10)]
    public string[] lignesIntro;
    public float delayBetweenLines = 2.5f;
    public float charDelay = 0.02f;

    private void Start()
    {
        textComponent.text = "";
        StartCoroutine(AfficherLignesUneParUne());
    }

    IEnumerator AfficherLignesUneParUne()
    {
        foreach (string ligne in lignesIntro)
        {
            yield return StartCoroutine(AfficherLettreParLettre(ligne));
            yield return new WaitForSeconds(delayBetweenLines);
            textComponent.text += "\n\n"; // Ajoute un espace entre les paragraphes
        }
    }

    IEnumerator AfficherLettreParLettre(string ligne)
    {
        textComponent.text = "";
        foreach (char c in ligne)
        {
            textComponent.text += c;
            yield return new WaitForSeconds(charDelay);
        }
    }
}
