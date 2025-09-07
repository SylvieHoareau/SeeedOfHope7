using UnityEngine;
using System.Collections.Generic;
using System;

public class ResourceUI : MonoBehaviour
{

    // On crée une classe pour stocker les associations ItemData/Prefab
    [Serializable]
    public class resourceSlotPrefab
    {
        public ItemData item;
        public GameObject prefab;
    }
    public Inventory inventory; // à lier dans l'inspecteur

    public ObjectiveManager objectiveManager;

    // Liste pour stocker les préfabriqués de slots
    public List<resourceSlotPrefab> resourceSlotPrefabs;

    // Le conteneur où les slots seront instanciés
    public Transform container;

    // Un dictionnaire pour garder une trace des slots déjà crées
    private Dictionary<ItemData, ResourceUISlot> resourceSlots = new Dictionary<ItemData, ResourceUISlot>();

    void Start()
    {
        // Tente de trouver l'inventaire
        if (inventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) inventory = player.GetComponent<Inventory>();
            if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
            if (inventory == null) Debug.LogWarning("ResourceUI: aucun Inventory trouvé. Assignez-le dans l'inspecteur.");
        }

        // Tente de trouver l'ObjectiveManager
        if (objectiveManager == null)
        {
            objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            if (objectiveManager == null) Debug.LogWarning("ResourceUI: aucun ObjectiveManager trouvé.");
        }

        // S'abonner à l'événement pour mettre à jour l'UI quand l'inventaire change
        inventory.onResourceChanged += UpdateUI;

        // Mettre à jour l'UI immédiatement au démarrage
        UpdateUI();
    }

    void OnDisable()
    {
        if (inventory != null)
        {
            inventory.onResourceChanged -= UpdateUI;
        }
    }

    // Méthode pour trouvé le bon préfabriqué
    private GameObject GetPrefabForItem(ItemData item)
    {
        foreach (var slotPrefab in resourceSlotPrefabs)
        {
            if (slotPrefab.item == item)
            {
                return slotPrefab.prefab;
            }
        }
        return null;
    }

      private int GetRequiredAmountForItem(ItemData item)
    {
        if (objectiveManager == null) return 0;
        foreach (var req in objectiveManager.requirements)
        {
            if (req.item == item)
            {
                return req.requiredAmount;
            }
        }
        return 0;
    }

    // Méthode centrée : met à jour les trois champs d'UI avec la forme "collecté / objectif"
    private void UpdateUI()
    {
        // Si l'une des références essentielles manque, on logge pour debug
        if (inventory == null || container == null)
        {
            Debug.LogWarning("ResourceUI: UpdateUI appelé mais Inventory est null.");
            return;
        }

        // On parcourt toutes les ressources de l'inventaire
        foreach (var resource in inventory.GetAllResources())
        {
            ItemData item = resource.Key;
            int currentCount = resource.Value;
            int requiredCount = GetRequiredAmountForItem(item); // On récupère le nombre requis

            // On vérifie si un slot existe déjà pour cette ressource
            if (!resourceSlots.ContainsKey(item))
            {
                // Si non, on crée un nouveau slot à partir du préfabriqué
                GameObject prefabToInstantiate = GetPrefabForItem(item);

                if (prefabToInstantiate != null)
                {
                    GameObject newSlotObject = Instantiate(prefabToInstantiate, container);
                    ResourceUISlot newSlot = newSlotObject.GetComponent<ResourceUISlot>();

                    if (newSlot != null)
                    {
                        // On configure et on ajoute le nouveau slot au dictionnaire
                        newSlot.Setup(item, currentCount, requiredCount);
                        resourceSlots.Add(item, newSlot);
                    }
                    else
                    {
                        Debug.LogWarning($"Le préfabriqué pour l'item {item.displayName} ne contient pas de composant ResourceUISlot");
                    }
                }
                else
                {
                    Debug.LogWarning($"Aucun préfabriqué n'est assigné pour l'item {item.displayName}.");
                }
            }
            else
            {
                // Si oui, on met simplement à jour le nombre affiché
                resourceSlots[item].UpdateCount(currentCount);
            }
        }
    }
}
