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
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioSource))]
public class AITerminal : MonoBehaviour
{
    // Référence au Scriptable Object
    // [Header("Configuration du niveau")]
    // public LevelData donneesDeNiveau;

    [Header("Inventaire du joueur")]
    // Référence à l'inventaire du joueur.
    // Contient les nombres d'objets que le joueur a ramassés (eau, graines, engrais).
    // Vous pouvez glisser ici l'objet "Player" dans l'inspecteur Unity pour le lier.
    public Inventory playerInventory;

    [Header("UI")]
    // Composant qui affiche les messages à l'écran (fenêtre, bulles de texte, etc.).

    public MessageManager messageManager;

    // La référence directe au LevelData est remplacée par la récupération via le GameManager.
    private LevelData donneesDeNiveau;

    [Header("Zones à revitaliser")]
    // Liste des objets/éléments de la scène qui seront activés quand
    // le terminal sera déclenché (ex : portes, lumières, mécanismes).
    public GameObject[] zonesARevitaliser;
    public GameObject porte;

    [Header("Dialogues")]
    // Contient plusieurs lignes de texte que l'IA peut prononcer.
    // Par exemple : introduction, ressources insuffisantes, succès, etc.
    public Dialogue dialogue;

    [Header("Audio")]
    private AudioSource audioSource;
    // Son joué quand l'activation réussit
    public AudioClip activationSound;
    // Son joué quand le joueur n'a pas assez de ressources
    public AudioClip ressourcesInsuffisantesSound;

    // Indique si le joueur est à portée pour interagir avec le terminal
    private bool joueurDansZone = false;
    // Indique si l'objectif principal a déjà été rempli
    private bool objectifAtteint = false;
    // Flags pour éviter d'afficher plusieurs fois le même dialogue
    private bool dialogueInitialAffiche = false;
    private bool dialogueObjectifAffiche = false;

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

        // Obtenir les données de niveau du GameManager au démarrage
        donneesDeNiveau = GameManager.Instance.GetCurrentLevelData();
        if (donneesDeNiveau == null)
        {
            Debug.LogError("AITerminal: Impossible d'obtenir les données de niveau du GameManager.");
            this.enabled = false; // Désactive le script pour éviter d'autres erreurs
            return;
        }
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

        // S'abonne à l'événement de changement d'inventaire
        if (playerInventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null) playerInventory = player.GetComponent<Inventory>();

            if (playerInventory == null)
            {
                Debug.LogWarning("[AITerminal] Aucun inventaire de joueur trouvé. Assignez l'Inventory du joueur dans l'inspecteur.");
            }
        }

        // On s'abonne à l'événement de changement d'inventaire
        if (playerInventory != null)
        {
            // Cela permet de mettre à jour l'état du terminal automatiquement
            // dès que le joueur ramasse quelque chose (sans attendre une interaction).
            playerInventory.onResourceChanged += CheckForObjectives;
        }
        
        // Affiche le message de bienvenue dès le lancement du niveau
        // Le dialogue.sentences[0] doit contenir le message de bienvenue dans l'inspecteur.
        if (messageManager != null && dialogue != null && dialogue.sentences.Length > 0)
        {
            // Affiche la première phrase d'introduction et la garde 20 secondes.
            messageManager.ShowMessage(dialogue.sentences[0], 20f); 
        }
    }

    void OnDestroy()
    {
        // On se désabonne pour éviter les erreurs
        if (playerInventory != null)
        {
            playerInventory.onResourceChanged -= CheckForObjectives;
        }
    }

    /// <summary>
    /// Vérifie si l'objectif est atteint et affiche le dialogue approprié.
    /// Ce dialogue est passif, il n'y a pas besoin d'interaction du joueur.
    /// </summary>
    private void CheckForObjectives()
    {
        // Si l'objectif est déjà atteint ou le dialogue déjà affiché, on ne fait rien
        if (objectifAtteint || dialogueObjectifAffiche || donneesDeNiveau == null) return;
        
        bool aToutesLesRessources = true;
        foreach(ResourceGoal goal in donneesDeNiveau.objectifs)
        {
            if (playerInventory.GetResourceCount(goal.type) < goal.amount)
            {
                aToutesLesRessources = false;
                break; // Sort de la boucle dès qu'un objectif n'est pas atteint
            }
        }
        
        // Si le joueur a toutes les ressources, on affiche le dialogue de succès
        if (aToutesLesRessources)
        {
            if (messageManager != null && donneesDeNiveau.dialogueObjectifAtteint != null)
            {
                messageManager.ShowMessage(donneesDeNiveau.dialogueObjectifAtteint.sentences[0]);
                dialogueObjectifAffiche = true; // évite de réafficher le même message
            }
        }
    }

    /// <summary>
    /// Logique pour l'interaction avec le terminal IA (quand le joueur appuie sur "E").
    /// </summary>
    void ActiverIA()
    {
        if (playerInventory == null || donneesDeNiveau == null) return;

        bool aToutesLesRessources = true;
        foreach(ResourceGoal goal in donneesDeNiveau.objectifs)
        {
            if (playerInventory.GetResourceCount(goal.type) < goal.amount)
            {
                aToutesLesRessources = false;
                break;
            }
        }
        
        if (objectifAtteint)
        {
            if (messageManager != null && donneesDeNiveau.dialogueObjectifDejaAtteint != null)
            {
                messageManager.ShowMessage(donneesDeNiveau.dialogueObjectifDejaAtteint.sentences[0]);
            }
            return;
        }
        else if (aToutesLesRessources)
        {
            // CAS DE SUCCÈS
            if (audioSource != null && activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }
            
           // On active les zones référencées dans ce script
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null) zone.SetActive(true);
            }

            // C'est ici que vous activez la porte !
            if (porte != null)
            {
                porte.SetActive(true);
            }
            
            objectifAtteint = true;
            this.enabled = false;

            if (messageManager != null && donneesDeNiveau.dialogueObjectifAtteint != null)
            {
                messageManager.ShowMessage(donneesDeNiveau.dialogueObjectifAtteint.sentences[0]);
            }
            
            GameManager.Instance.LoadNextLevel();
        }
        else
        {
            // CAS D'ÉCHEC
            if (messageManager != null && donneesDeNiveau.dialogueObjectifEchec != null)
            {
                messageManager.ShowMessage(donneesDeNiveau.dialogueObjectifEchec.sentences[0]);
            }
            
            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Le joueur est entré dans la zone d'interaction du terminal.
            joueurDansZone = true;

            // Si l'introduction n'a pas encore été affichée pour ce terminal,
            // on la montre (ex : "Bienvenue, explorateur...").
            if (!dialogueInitialAffiche && messageManager != null && donneesDeNiveau.dialogueBienvenue != null)
            {
                // Affiche la première phrase d'introduction
                messageManager.ShowMessage(donneesDeNiveau.dialogueBienvenue.sentences[0]);
                dialogueInitialAffiche = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Le joueur quitte la zone d'interaction ; on cache le panneau éventuel.
            joueurDansZone = false;
            
            // Vérifie si la référence est valide avant d'y accéder.
            if (!dialogueInitialAffiche && messageManager != null && donneesDeNiveau != null && donneesDeNiveau.dialogueBienvenue != null)
            {
                messageManager.ShowMessage(donneesDeNiveau.dialogueBienvenue.sentences[0]);
                dialogueInitialAffiche = true;
            }
        }
    }
}