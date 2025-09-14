using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Ce script contrôle l'affichage d'un seul élément d'UI de ressource
public class ResourceUIElement : MonoBehaviour
{
    // On rend les références publiques pour qu'elles puissent être assignées dans l'éditeur
    public Image icon;
    public TextMeshProUGUI resourceText;

    // Met à jour l'icône et le texte d'un coup
    public void UpdateUI(Sprite resourceIcon, string text)
    {
        if (icon != null)
        {
            icon.sprite = resourceIcon;
        }
        if (resourceText != null)
        {
            resourceText.text = text;
        }
    }
}