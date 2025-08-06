using UnityEngine;
using TMPro;

public class IntroMessage : MonoBehaviour
{
    public TextMeshProUGUI messageUI;
    public float affichageDuree = 5f;

    public string message =
        "[IA LOG]" +
        "Collectez de l'eau, des graines et de l'engrais pour restaurer la terre.\n" +
        "Interragisez avec moi (E) lorsque vous êtes prêt. \n" +
        "Utilise les flèches du clavier pour te déplacer";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (messageUI != null)
        {
            messageUI.text = message;
            Invoke(nameof(CacherMessage), affichageDuree);
        }
        else
        {
            Debug.LogWarning("IntroMessage : messageUI non assigné !");
        }
    }

    void CacherMessage()
    {
        if (messageUI != null)
        {
            messageUI.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
