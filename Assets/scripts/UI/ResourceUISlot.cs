using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Ce script gère l'affichage d'une seule ressource
public class ResourceUISlot : MonoBehaviour
{
    // Image qui représente l'icône de la ressource
    public Image iconImage;
    // Texte qui affiche le nombre de ressources possédées
    public TextMeshProUGUI countText;

    // Inofmrations sur la ressources à afficher
    private ItemData item;
    private int requiredAmount; // Pour stocker la quantité requise

    // Cette fonction permet de mettre à jour l'affichage du slot avec une nouvelle ressource et sa quantité
    public void Setup(ItemData newItem, int count, int required)
    {
        // On garde en mémoire la ressource à afficher
        item = newItem;
        requiredAmount = required; // Stocke la quantité requise

        // Si une icône existe pour la ressource, on l'affiche
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon; // On met l'image de la ressource
            iconImage.enabled = true; // on rend l'imge visible
        }
        // Si aucune icône n'est disponible, on cache l'image
        else if (iconImage != null)
        {
            iconImage.enabled = false; // Cacher l'image s'il n'y a pas de sprite
        }

        // On met à jour le texte pour afficher la quantité de la ressource
        UpdateCount(count);
    }

    // Cette fonction met à jour uniquement le nombre affiché (utile si la quantité change)
    public void UpdateCount(int newCount)
    {
        // Si le composant texte existe, on affiche le nouveau nombre
        if (countText != null)
        {
            // Affichage de la quantité actuelle ET de la quantité requise
            countText.text = $"{newCount} / {requiredAmount}";
        }
    }
}