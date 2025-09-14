// Ce script gère l'inventaire du joueur, en gardant une trace des ressources collectées.
// Il utilise un dictionnaire pour une gestion flexible des différents types d'objets.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// L'enum (énumération) nous permet de définir des types de ressources
// prédéfinis pour éviter les erreurs de frappe.
public enum ResourceType
{
    WaterDrop,
    Seed,
    Fertilizer,
    Flower,
    Bee,
    Mushroom
}

public class Inventory : MonoBehaviour
{
    // Le dictionnaire qui stocke les ressources.
    // La "clé" est le type de ressource, et la "valeur" est la quantité.
    private Dictionary<ResourceType, int> resources = new Dictionary<ResourceType, int>();

    // Cette action permet de prévenir d'autres scripts quand une ressource est ajoutée
    public event Action onResourceChanged;

    void Start()
    {
        // On initialise l'inventaire en ajoutant les types de ressources
        // avec une quantité de 0 pour commencer.
        foreach (ResourceType type in Enum.GetValues(typeof(ResourceType)))
        {
            resources[type] = 0;
        }
    }

    /// <summary>
    /// Ajoute une ressource à l'inventaire du joueur.
    /// Cette fonction est appelée par le script "Pickup".
    /// </summary>
    /// <param name="type">Le type de ressource à ajouter (ex: WaterDrop, Seed)</param>
    /// <param name="amount">La quantité à ajouter</param>
    public void AddResource(ResourceType type, int amount)
    {
        // On s'assure que la quantité est positive
        if (amount <= 0) return;

        // On vérifie si la ressource est déjà dans le dictionnaire
        if (resources.ContainsKey(type))
        {
            // Si oui, on ajoute la nouvelle quantité à l'ancienne
            resources[type] += amount;
        }
        else
        {
            // Sinon, on ajoute la nouvelle ressource avec la quantité donnée
            resources[type] = amount;
        }

        Debug.Log($"[Inventaire] {type} ajouté. Quantité actuelle : {resources[type]}");

        // On déclenche l'événement pour que les autres scripts soient informés du changement
        onResourceChanged?.Invoke();
    }

    // Vous pouvez ajouter d'autres fonctions pour obtenir le compte des ressources si vous en avez besoin.
    // Par exemple, pour que le Terminal IA puisse vérifier si l'objectif est atteint.

    /// <summary>
    /// Retourne le nombre d'unités d'une ressource donnée.
    /// </summary>
    /// <param name="type">Le type de ressource à vérifier.</param>
    /// <returns>La quantité de la ressource dans l'inventaire.</returns>
    public int GetResourceCount(ResourceType type)
    {
        // Utilise la méthode GetValueOrDefault pour retourner 0 si la clé n'existe pas
        return resources.GetValueOrDefault(type, 0);
    }
    
    // public int GetWaterDropCount()
    // {
    //     return resources.GetValueOrDefault(ResourceType.WaterDrop, 0);
    // }

    // public int GetSeedCount()
    // {
    //     return resources.GetValueOrDefault(ResourceType.Seed, 0);
    // }

    // public int GetFertilizerCount()
    // {
    //     return resources.GetValueOrDefault(ResourceType.Fertilizer, 0);
    // }
}
