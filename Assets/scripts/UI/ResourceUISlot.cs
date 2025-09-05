using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ce script gère l'affichage d'une seule ressource
public class ResourceUISlot : MonoBehaviour
{
    // Références aux composants UI dans le GameObject parent
    public Image iconImage;
    public TextMeshProUGUI countText;

    // La référence à l'ItemData que ce slot doit afficher
    private ItemData item;

    // Met à jour le slot avec une nouvelle ressource et sa quantité
    public void Setup(ItemData newItem, int count)
    {
        item = newItem;

        // Si l'icône existe, on l'affiche
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true; // S'assurer que l'image est visible
        }
        else if (iconImage != null)
        {
            iconImage.enabled = false; // Cacher l'image s'il n'y a pas de sprite
        }

        // Mettre à jour le texte avec la quantité
        UpdateCount(count);
    }

    // Met à jour uniquement la quantité, utile quand le joueur en
    public void UpdateCount(int newCount)
    {
        if (countText != null)
        {
            countText.text = $"{newCount}";
        }
    }
}