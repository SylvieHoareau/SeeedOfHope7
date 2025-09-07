using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

// Manager Central qui va s'assurer que le joueur a collecté toutes les ressources nécessaires pour le niveau 
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
    public TextMeshProUGUI terminalText;

    [Header("Porte de sortie")]
    public GameObject exitPortal;

    [Header("Sons")]
    public AudioClip victorySound;
    public AudioSource audioSource; // pour jouer le son

    public bool objectivesCompleted = false;

    // Au démarrage du jeu
    void Start()
    {
        // On s'assure d'avoir l'inventaire
        if (inventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) inventory = player.GetComponent<Inventory>();
            if (inventory == null)
            {
                inventory = FindFirstObjectByType<Inventory>();
                Debug.LogWarning("ResourceUI: aucun Inventory trouvé.");
            }
        }

        // On s'abonne à l'événement de changement d'inventaire
            if (inventory != null)
            {
                inventory.onResourceChanged += CheckObjectivesAndUI;
            }

        // On vérifie une fois au démarrage pour intialiser l'UI
        CheckObjectivesAndUI();
    }

    // Se désabonner pour éviter les fuites de mémoire et les erreurs quand le script est désactivé
    private void OnDisable()
    {
        if (inventory != null)
        {
            inventory.onResourceChanged -= CheckObjectivesAndUI;
        }
    }

    // On remplace la logique de l'Update par une méthode déclenchée
    private void CheckObjectivesAndUI()
    {
        // Si les objactifs sot déjà complétés, on ne fait rien
        if (objectivesCompleted) return;

        bool allObjectivesMet = true;
        string objectiveDisplay = "Objectifs :\n";

        // Met à jour le texte de l'UI
        // objectiveText.text = objectiveDisplay;

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
            }
        }
        // Met à jour le texte de l'UI en une seule fois
        if (objectiveText != null)
        {
            objectiveText.text = objectiveDisplay;
        }

        // SI tous les objectifs sont atteints, on déclenche la logique
        if (allObjectivesMet)
        {
            objectivesCompleted = true;
            OnObjectivesCompleted();
        }
    }

    // Cette méthode sera appelée quand les objectifs sont complétés
    private void OnObjectivesCompleted()
    {
        Debug.Log("Tous les objectifs atteints !");


        // Changer le message du terminal IA
        if (terminalText != null)
        {
            // Lance la coroutine pour afficher un message temporaire
            StartCoroutine(ShowTerminalMessage("Objectifs atteints. Trouver la porte de sortie.", 10f));
        }

        // Activer la porte de sortie
        if (exitPortal != null)
        {
            exitPortal.SetActive(true);
        }

        // Jouer un son de victoire
        if (audioSource != null && victorySound != null)
        {
            audioSource.PlayOneShot(victorySound);
        }

    }

    // Coroutine pour afficher un message sur le terminal pendant un court laps de temps
    IEnumerator ShowTerminalMessage(string message, float duration)
    {
        terminalText.text = message;
        yield return new WaitForSeconds(duration);
        terminalText.text = "";
    }

}
