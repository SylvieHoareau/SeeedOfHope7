using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ResourceUI : MonoBehaviour
{
    [Header("Références")]
    // La référence à l'inventaire du joueur
    public Inventory inventory;
    // Le conteneur dans l'UI où seront instanciés les éléments de ressource
    public Transform contentParent; // Parent des éléments d'UI des ressources

    [Header("Prévisualisation de l'élément d'UI")]
    // Le préfab pour chaque élément d'affichage de ressource
    public GameObject resourceUIPrefab; // Le préfab de l'élément d'UI d'une ressource

    // Dictionnaire pour lier les types de ressources aux Texte de l'UI
    private Dictionary<ResourceType, TextMeshProUGUI> resourceTexts = new Dictionary<ResourceType, TextMeshProUGUI>();
    // Référence au terminal IA
    public AITerminal terminalIA;
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

            if (inventory == null)
            {
                Debug.Log("ResourceUI: Inventory trouvé sur l'objet Player.");
                return;
            }
        }

        // Initialisation dynamique de l'UI en fonction des objectifs du niveau
        InitializeUI();

       // 3. S'abonne à l'événement de l'inventaire
        if (inventory != null)
        {
            inventory.onResourceChanged += UpdateUI;
            // Met à jour l'UI une première fois
            UpdateUI();
        }

        if (terminalIA == null)
        {
            terminalIA = FindFirstObjectByType<AITerminal>();
            if (terminalIA == null)
                Debug.LogWarning("ResourceUI: aucun AITerminal trouvé. Assignez-le dans l'inspecteur.");
            else
                Debug.Log("ResourceUI: AITerminal trouvé automatiquement.");
        }
    }

    /// <summary>
    /// Initialise dynamiquement l'UI en créant un élément pour chaque objectif du niveau.
    /// </summary>
    void InitializeUI()
    {
        // Obtenir les données de niveau via le GameManager
        LevelData currentLevelData = GameManager.Instance.GetCurrentLevelData();
        if (currentLevelData == null || currentLevelData.objectifs == null)
        {
            Debug.LogError("ResourceUI: Les données de niveau ou les objectifs sont manquants !");
            return;
        }

        // Supprimer les anciens éléments d'UI s'il y en a
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }
        resourceTexts.Clear();

        // Créer un élément d'UI pour chaque ressource demandée par le niveau
        foreach (ResourceGoal goal in currentLevelData.objectifs)
        {
            GameObject uiElement = Instantiate(resourceUIPrefab, contentParent);
            TextMeshProUGUI textComponent = uiElement.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                // Stocker la référence pour une mise à jour facile plus tard
                resourceTexts.Add(goal.type, textComponent);
            }
        }
    }

    void OnDisable()
    {
        // Se désabonne proprement pour éviter les erreurs
        if (inventory != null)
        {
            inventory.onResourceChanged -= UpdateUI;
        }
    }

    
    /// <summary>
    /// Met à jour le texte de l'UI avec les valeurs actuelles de l'inventaire.
    /// </summary>
    private void UpdateUI()
    {
        // Vérification de la validité de l'inventaire
        if (inventory == null) return;

        // Obtenir les données de niveau à jour
        LevelData currentLevelData = GameManager.Instance.GetCurrentLevelData();
        if (currentLevelData == null || currentLevelData.objectifs == null) return;

        // Parcourir les objectifs pour mettre à jour chaque élément d'UI
        foreach (ResourceGoal goal in currentLevelData.objectifs)
        {
            if (resourceTexts.ContainsKey(goal.type))
            {
                int collectedAmount = inventory.GetResourceCount(goal.type);
                string formattedName = FormatResourceName(goal.type.ToString());

                resourceTexts[goal.type].text = $"{formattedName} : {collectedAmount} / {goal.amount}";
            }
        }
    }
    
    /// <summary>
    /// Aide pour formater le nom de l'enum pour l'UI.
    /// </summary>
    private string FormatResourceName(string name)
    {
        // Ex: "WaterDrop" -> "Eau"
        switch (name)
        {
            case "WaterDrop":
                return "Eau";
            case "Seed":
                return "Graines";
            case "Fertilizer":
                return "Engrais";
            case "Flower":
                return "Fleurs";
            case "Bee":
                return "Abeille";
            case "Mushroom":
                return "Champignon";
            default:
                return name;
        }
    }
}
