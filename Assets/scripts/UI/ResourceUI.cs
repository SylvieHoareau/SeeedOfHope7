using UnityEngine;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{
    public Inventory inventory; // à lier dans l'inspecteur

    // Le préfab de l'UI d'un seul solt de ressource
    public GameObject resourceSlotPrefab;

    // Le conteneur où les slots seront instanciés
    public Transform container;

    // Un dictionnaire pour garder une trace des slots déjà crées
    private Dictionary<ItemData, ResourceUISlot> resourceSlots = new Dictionary<ItemData, ResourceUISlot>();

    void Start()
    {
        // Si une référence n'est pas assignée dans l'inspecteur, on tente de la trouver automatiquement
        if (inventory == null)
        {
            // Cherche d'abord l'inventory sur le GameObject taggé "Player" (recommandé)
            var player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<Inventory>();
            }

            // Si pas trouvé via le tag, on essaie la méthode générique
            if (inventory == null)
            {
                inventory = FindFirstObjectByType<Inventory>();
            }
            if (inventory == null)
            {
                Debug.LogWarning("ResourceUI: aucun Inventory trouvé. Assignez-le dans l'inspecteur.");
                return;
            }
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

    // Méthode centrée : met à jour les trois champs d'UI avec la forme "collecté / objectif"
    private void UpdateUI()
    {
        // Si l'une des références essentielles manque, on logge pour debug
        if (inventory == null)
        {
            Debug.LogWarning("ResourceUI: UpdateUI appelé mais Inventory est null.");
            return;
        }

        // On parcourt toutes les ressources de l'inventaire
        foreach (var resource in inventory.GetAllResources())
        {
            ItemData item = resource.Key;
            int count = resource.Value;

            // On vérifie si un slot existe déjà pour cette ressource
            if (!resourceSlots.ContainsKey(item))
            {
                // Si non, on crée un nouveau slot à partir du préfabriqué
                GameObject newSlotObject = Instantiate(resourceSlotPrefab, container);
                ResourceUISlot newSlot = newSlotObject.GetComponent<ResourceUISlot>();

                // On configure et on ajoute le nouveau slot au dictionnaire
                newSlot.Setup(item, count);
                resourceSlots.Add(item, newSlot);
            }
            else
            {
                // Si oui, on met simplement à jour le nombre affiché
                resourceSlots[item].UpdateCount(count);
            }
        }
    }
}
