using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;


public class ObjectiveManager : MonoBehaviour
{
    // On réutilise la classe de données que nous avons créés
    [System.Serializable]
    public class ResourceRequirement
    {
        public ItemData item;
        public int requiredAmount;
    }

    [Header("Inventaire")]
    public Inventory inventory; // à lier dans l'inspecteur

    [Header("UI Objectifs")]
    public TextMeshProUGUI objectiveText;

    [Header("Objectifs requis")]
    // La liste qui contiendra tous les objectifs du niveau
    public List<ResourceRequirement> requirements;

    [Header("Terminal IA")]
    public TextMeshProUGUI terminalText; // texte affiché par le Terminal

    [Header("Porte de sortie")]
    public GameObject exitPortal;

    private bool objectivesCompleted = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // On s'assure d'avoir l'inventaire
        if (inventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) inventory = player.GetComponent<Inventory>();
        }

        // On s'abonne à l'événement de changement d'inventaire
        if (inventory != null)
        {
            inventory.onResourceChanged += CheckObjectivesAndUI;
        }

        // On vérifie une fois au démarrage pour intialiser l'UI
        CheckObjectivesAndUI();
    }

    // Se désabonner pour éviter les fuites de mémoire
    private void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.onResourceChanged -= CheckObjectivesAndUI;
        }
    }

    // On remplace la logique de l'Update par une méthode déclenchée
    private void CheckObjectivesAndUI()
    {
        if (inventory == null || objectiveText == null || objectivesCompleted) return;

        // Chaîne de caractères pour construire le texte d'objectifs
        string objectiveDisplay = "Objectifs:\n";
        bool allObjectivesMet = true;

        // On parcourt la liste des objectifs requis
        foreach (ResourceRequirement req in requirements)
        {
            if (req.item != null)
            {
                int currentAmount = inventory.GetResourceCount(req.item);

                // On ajoute une ligne au texte
                objectiveDisplay += $"{req.item.displayName} : {currentAmount} / {req.requiredAmount}\n";

                // Si un seul objectif n'est pas atteint, on met le flag à 
                if (currentAmount < req.requiredAmount)
                {
                    allObjectivesMet = false;
                }

                // Met à jour le texte de l'UI
                objectiveText.text = objectiveDisplay;

                // Si tous les objectifs sont atteints, on déclenche la logique
                if (allObjectivesMet)
                {
                    objectivesCompleted = true;
                    OnObjectivesCompleted();
                }
            }
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
