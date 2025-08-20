using UnityEngine;
using TMPro;
using System.Collections;
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

    [Header("Terminal IA")]
    public TextMeshProUGUI terminalText; // texte affiché par le Terminal

    [Header("Porte de sortie")]
    public GameObject exitPortal;

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
        // 1. Message du Terminal IA 
        if (terminalText != null)
        {
            terminalText.text = "Objectifs atteints. Trouver la porte de sortie.";
        }

        // 2. Activer la porte de sortie (téléporteur)
        if (exitPortal != null)
        {
            exitPortal.SetActive(true);
        }

        StartCoroutine(ShowTerminalMessage("Objectifs atteints. Trouver la porte de sortie.", 5f));

    }

    IEnumerator ShowTerminalMessage(string message, float duration)
    {
        terminalText.text = message;
        yield return new WaitForSeconds(duration);
        terminalText.text = "";
    }
}
