using UnityEngine;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;

public class ObjectiveManager : MonoBehaviour
{
    [Header("Inventaire")]
    public Inventory inventory; // à lier dans l'inspecteur

    [Header("UI Objectifs")]
    public TextMeshProUGUI objectiveText;

    [Header("Objectifs requis")]
    public int requiredWater = 5;
    public int requiredSeeds = 10;
    public int requiredFertilizer = 3;

    private bool objectivesCompleted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (inventory == null || objectiveText == null) return;

        // Vérifie la progression
        int currentWater = inventory.GetWaterDropCount();
        int currentSeeds = inventory.GetSeedCount();
        int currentFertilizer = inventory.GetFertilizerCount();

        // Met à jour le texte
        objectiveText.text =
            "Objectifs :\n" +
            "Eau : " + currentWater + " / " + requiredWater + "\n" +
            "Graines : " + currentSeeds + " / " + requiredSeeds + "\n" +
            "Engrais : " + currentFertilizer + " / " + requiredFertilizer;

        // Vérifie si tout est atteint
        if (!objectivesCompleted &&
            currentWater >= requiredWater &&
            currentSeeds >= requiredSeeds &&
            currentFertilizer >= requiredFertilizer)
        {
            objectivesCompleted = true;
            OnObjectivesCompleted();
        }
    }
    
    void OnObjectivesCompleted()
    {
        Debug.Log("Tous les objectifs atteints !");
        // Ici tu peux lancer la prochaine étape du jeu :
        // - Débloquer une cinématique
        // - Changer de scène
        // - Afficher un message de victoire
    }
}
