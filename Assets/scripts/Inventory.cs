// Ce script gère l'inventaire du joueur, en gardant une trace des ressources collectées.
// Il utilise un dictionnaire pour une gestion flexible des différents types d'objets.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Inventory : MonoBehaviour
{
    // Le dictionnaire stocke maintenant des ItemData
    // comme clés, et des entiers pour les quantités
    private Dictionary<ItemData, int> resources = new Dictionary<ItemData, int>();

    // Cette liste sera affichée dans l'inspecteur Unity pour confirmer
    [SerializeField]
    private List<ItemData> initialResources = new List<ItemData>();

    // Cette action permet de prévenir d'autres scripts quand une ressource est ajoutée
    public event Action onResourceChanged;

    void Start()
    {
        // On initialise l'inventaire en se basant sur la liste
        InitializeInventory();
    }

    private void InitializeInventory()
    {
        foreach (ItemData item in initialResources)
        {
            if (item != null && !resources.ContainsKey(item))
            {
                resources[item] = 0;
            }
        }
    }

    /// <summary>
    /// Ajoute une ressource à l'inventaire du joueur.
    /// Cette fonction est appelée par le script "Pickup".
    /// </summary>
    /// <param name="type">Le type de ressource à ajouter (ex: WaterDrop, Seed)</param>
    /// <param name="amount">La quantité à ajouter</param>
    public void AddResource(ItemData item, int amount)
    {
        // On s'assure que la quantité est positive
        if (amount <= 0) return;

        // On vérifie si la ressource est déjà dans le dictionnaire
        if (resources.ContainsKey(item))
        {
            // Si oui, on ajoute la nouvelle quantité à l'ancienne
            resources[item] += amount;
        }
        else
        {
            // Sinon, on ajoute la nouvelle ressource avec la quantité donnée
            resources[item] = amount;
        }

        Debug.Log($"[Inventaire] {item} ajouté. Quantité actuelle : {resources[item]}");

        // On déclenche l'événement pour que les autres scripts soient informés du changement
        onResourceChanged?.Invoke();
    }

    /// <summary>
    /// Obtient la quantité d'une ressource
    /// </summary>
    /// <param name="item">L'itemData dont on veut la quantité.</param>
    public int GetResourceCount(ItemData item)
    {
        return resources.GetValueOrDefault(item, 0);
    }

    /// <summary>
    /// Retourne le dictionnaire des ressources pour un affichage 
    /// </summary>
    /// <returns></returns>
    public Dictionary<ItemData, int> GetAllResources()
    {
        return resources;
    }
}
