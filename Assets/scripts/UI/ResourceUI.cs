using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public Inventory inventory; // à lier dans l'inspecteur
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI fertilizerText;
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
                if (inventory != null)
                    Debug.Log("ResourceUI: Inventory trouvé sur l'objet Player.");
            }

            // Si pas trouvé via le tag, on essaie la méthode générique
            if (inventory == null)
                inventory = FindFirstObjectByType<Inventory>();
            if (inventory == null)
                Debug.LogWarning("ResourceUI: aucun Inventory trouvé. Assignez-le dans l'inspecteur.");
            else
                Debug.Log("ResourceUI: Inventory trouvé automatiquement.");
        }

        if (terminalIA == null)
        {
            terminalIA = FindFirstObjectByType<AITerminal>();
            if (terminalIA == null)
                Debug.LogWarning("ResourceUI: aucun AITerminal trouvé. Assignez-le dans l'inspecteur.");
            else
                Debug.Log("ResourceUI: AITerminal trouvé automatiquement.");
        }

        // S'abonner à l'événement pour mettre à jour l'UI quand l'inventaire change
        if (inventory != null)
            inventory.onResourceChanged += UpdateUI;

        // Mettre à jour l'UI immédiatement au démarrage
        UpdateUI();
    }

    void OnDisable()
    {
        if (inventory != null)
            inventory.onResourceChanged -= UpdateUI;
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
        if (terminalIA == null)
        {
            Debug.LogWarning("ResourceUI: UpdateUI appelé mais AITerminal est null.");
            return;
        }

        int w = inventory.GetWaterDropCount();
        int s = inventory.GetSeedCount();
        int f = inventory.GetFertilizerCount();

        if (waterText != null)
            waterText.text = $"Eau : {w} / {terminalIA.besoinEau}";

        if (seedText != null)
            seedText.text = $"Graines : {s} / {terminalIA.besoinGraines}";

        if (fertilizerText != null)
            fertilizerText.text = $"Engrais : {f} / {terminalIA.besoinFertilisant}";

        Debug.Log($"ResourceUI: UI mis à jour -> Eau:{w}/{terminalIA.besoinEau} Graines:{s}/{terminalIA.besoinGraines} Engrais:{f}/{terminalIA.besoinFertilisant}");
    }
}
