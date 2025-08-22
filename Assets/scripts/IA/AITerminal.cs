// Ce script gère le terminal d'intelligence artificielle (IA) dans le jeu.
// Il gère la détection des ressources, les interactions et l'affichage des dialogues.

using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioSource))]
public class AITerminal : MonoBehaviour
{
    [Header("Inventaire du joueur")]
    public Inventory playerInventory;

    [Header("Zones à revitaliser")]
    public GameObject[] zonesARevitaliser;

    [Header("UI")]
    public MessageManager messageManager;

    [Header("Dialogues")]
    public Dialogue dialogue;

    [Header("Ressources requises")]
    public int besoinEau;
    public int besoinGraines;
    public int besoinFertilisant;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip activationSound;
    public AudioClip ressourcesInsuffisantesSound;

    private bool joueurDansZone = false;
    private bool objectifAtteint = false;
    private bool dialogueInitialAffiche = false;
    private bool dialogueObjectifAffiche = false;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        audioSource = GetComponent<AudioSource>();
        if (audioSource != null)
            audioSource.playOnAwake = false;
    }

    private void OnEnable()
    {
        controls.Player.Interact.performed += OnInteractPerformed;
        controls.Player.Interact.Enable();
    }

    private void OnDisable()
    {
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
            playerInventory.onResourceChanged += CheckForObjectives;
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
        if (objectifAtteint || dialogueObjectifAffiche) return;

        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        bool aToutesLesRessources = eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant;

        // Si le joueur a toutes les ressources, on affiche le dialogue 3
        if (aToutesLesRessources)
        {
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 2)
            {
                // Affiche le 3ème dialogue
                messageManager.ShowMessage(dialogue.sentences[2]);
                dialogueObjectifAffiche = true;
            }
        }
    }

    /// <summary>
    /// Logique pour l'interaction avec le terminal IA (quand le joueur appuie sur "E").
    /// </summary>
    void ActiverIA()
    {
        if (playerInventory == null) return;
        
        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        bool aToutesLesRessources = eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant;

        // Si l'objectif est déjà atteint, on affiche le dialogue 4 et on sort
        if (objectifAtteint)
        {
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 3)
            {
                // Affiche le 4ème dialogue
                messageManager.ShowMessage(dialogue.sentences[3]);
            }
            return;
        }
        // Sinon, si les ressources sont suffisantes (on vient de le vérifier)
        else if (aToutesLesRessources)
        {
            // On joue le son de succès
            if (audioSource != null && activationSound != null)
            {
                audioSource.PlayOneShot(activationSound);
            }
            
            // On active les zones
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null) zone.SetActive(true);
            }
            
            // On met à jour le statut de l'objectif
            objectifAtteint = true;
            
            // On désactive ce script, car le terminal a déjà été activé
            this.enabled = false;

            // Affiche le 4ème dialogue
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 3)
            {
                messageManager.ShowMessage(dialogue.sentences[3]);
            }
        }
        else
        {
            // Sinon, les ressources sont insuffisantes, on affiche le dialogue 2
            if (messageManager != null && dialogue != null && dialogue.sentences.Length > 1)
            {
                messageManager.ShowMessage(dialogue.sentences[1]);
            }
            
            // On joue le son d'échec
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
            joueurDansZone = true;
            // On affiche le dialogue 1 si ce n'est pas déjà fait
            if (!dialogueInitialAffiche && messageManager != null && dialogue != null && dialogue.sentences.Length > 0)
            {
                // Affiche le 1er dialogue (d'introduction)
                messageManager.ShowMessage(dialogue.sentences[0]);
                dialogueInitialAffiche = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            if (messageManager != null) messageManager.HidePanel();
        }
    }
}