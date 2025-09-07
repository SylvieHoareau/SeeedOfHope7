// AITerminal.cs
// -----------------
// Explication simple (pour non-développeur) :
// Ce script contrôle un terminal dans le jeu où l'IA peut "réveiller"
// certaines parties du niveau si le joueur a collecté assez de ressources.
// En pratique :
// - Le joueur collecte des objets (eau, graines, engrais) dans l'environnement.
// - L'inventaire du joueur est vérifié par ce terminal.
// - Si le joueur a assez de ressources, le terminal active des zones (ex : portes,
//   mécanismes, lumières), joue un son et affiche un message à l'écran.
// Les commentaires ci-dessous expliquent chaque section en termes non techniques.

using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;

// [System.Serializable]
// public class ResourceRequirement
// {
//     public ItemData item;
//     public int requiredAmount;
// }

[RequireComponent(typeof(AudioSource))]
public class AITerminal : MonoBehaviour
{
    [Header("Manager")]
    public ObjectiveManager objectiveManager;

    [Header("Dialogue et UI")]
    public MessageManager messageManager;
    public Dialogue dialogue;

    [Header("Porte de sortie")]
    public GameObject porte;
    public AudioClip activationSound;
    public AudioClip ressourcesInsuffisantesSound;

    public AudioSource audioSource;

    // Indique si le joueur est à portée pour interagir avec le terminal
    private bool joueurDansZone = false;

    // Objet généré par le système d'Input (clavier/manette).
    // Permet de détecter quand le joueur appuie sur la touche d'interaction.
    private PlayerControls controls;

    private void Awake()
    {
        // Prépare le système de contrôle (pour lire les actions du joueur)
        controls = new PlayerControls();

        // Récupère le composant AudioSource attaché à ce GameObject
        // (utilisé pour jouer des sons lorsque l'on active le terminal)
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.playOnAwake = false; // empêche le son de démarrer automatiquement
    }

    private void OnEnable()
    {
        // S'abonne à l'action d'interaction (par ex. le bouton "Interagir")
        controls.Player.Interact.performed += OnInteractPerformed;
        controls.Player.Interact.Enable();
    }

    private void OnDisable()
    {
        // Se désabonne proprement quand l'objet n'est plus actif
        controls.Player.Interact.performed -= OnInteractPerformed;
        controls.Player.Interact.Disable();
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (joueurDansZone)
        {
            ActiverIA();
        }
    }

    void Start()
    {

        // Affiche le message de bienvenue dès le lancement du niveau
        // Le dialogue.sentences[0] doit contenir le message de bienvenue dans l'inspecteur.
        if (messageManager != null && dialogue != null && dialogue.sentences.Length > 0)
        {
            // Affiche la première phrase d'introduction et la garde 20 secondes.
            messageManager.ShowMessage(dialogue.sentences[0], 20f);
        }

        // On trouve l'ObjectiveManager s'il n'est pas assigné
        if (objectiveManager == null)
        {
            objectiveManager = FindFirstObjectByType<ObjectiveManager>();
            if (objectiveManager == null)
            {
                Debug.LogWarning("[AITerminal] Aucun ObjectiveManager trouvé. Assurez-vous qu'il y en a un dans la scène.");
            }
        }

    }

    /// <summary>
    /// Logique pour l'interaction avec le terminal IA (quand le joueur appuie sur "E").
    /// </summary>
    void ActiverIA()
    {
        if (objectiveManager == null) return;
        
        // Si l'objectif est déjà atteint, on affiche le dialogue 4 et on sort
        if (objectiveManager.objectivesCompleted)
        {
            Debug.Log("Terminal utilisé, objectifs atteints !");

            // Jouer le son de succès
            if (audioSource != null && activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }

            // Activer la porte de sortie
            if (porte != null) porte.SetActive(true);
            
            // Affiche le dialogue de succès
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 3)
            {
                // Affiche le 4ème dialogue
                messageManager.ShowMessage(dialogue.sentences[3]);
            }
            return;
        }
        else
        {
            // CAS D'ÉCHEC : Le joueur n'a pas encore toutes les ressources requises.
            // On informe le joueur et on joue un son d'alerte si disponible.
              // Jouer le son d'échec
            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
            }

            // Sinon, les ressources sont insuffisantes, on affiche le dialogue 2
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 1)
            {
                messageManager.ShowMessage(dialogue.sentences[1]);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Le joueur est entré dans la zone d'interaction du terminal.
            joueurDansZone = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Le joueur quitte la zone d'interaction ; on cache le panneau éventuel.
            joueurDansZone = false;
            if (messageManager != null) messageManager.HidePanel();
        }
    }
}