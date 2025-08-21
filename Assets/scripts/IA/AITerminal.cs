
// Ce script gère le terminal d'intelligence artificielle (IA) dans le jeu.
// Il permet au joueur d'utiliser des ressources pour revitaliser des zones, avec des sons et des messages d'interface.
// Les commentaires sont adaptés pour être compris par tous, même sans connaissances en programmation !

using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System; // Ajout de la bibliothèque pour les coroutines

[RequireComponent(typeof(AudioSource))]
public class AITerminal : MonoBehaviour
{
    // L'inventaire du joueur (ce qu'il possède)
    public Inventory playerInventory;

    [Header("Zones à revitaliser")]
    // Liste des zones du jeu qui peuvent être "revitalisées" (réactivées) par l'IA
    public GameObject[] zonesARevitaliser;

    [Header("UI")]
    // Message affiché à l'écran pour informer le joueur
    public MessageManager messageManager;
    // Pour afficher l'objectif en continu
    public TextMeshProUGUI objectifText;

    // --- NOUVELLE SECTION POUR LES DIALOGUES ---
    [Header("Dialogues")]
    [TextArea(3,10)]
    public List<String> dialogue = new();

    [Header("Ressources requises")]
    // Nombre d'unités d'eau nécessaires pour activer l'IA
    public int besoinEau;
    // Nombre de graines nécessaires
    public int besoinGraines;
    // Nombre de fertilisant nécessaires
    public int besoinFertilisant;

    [Header("Audio")]
    // Permet de jouer des sons dans le jeu
    private AudioSource audioSource;
    // Son joué quand l'IA est activée avec succès
    public AudioClip iaInteractionSound;
    // Son joué si le joueur n'a pas assez de ressources
    public AudioClip ressourcesInsuffisantesSound;

    // Indique si le joueur est proche du terminal IA
    private bool joueurDansZone = false;

    // Booléen pour savoir si l'objectif est atteint
    private bool objectifAtteint = false;

    // Contrôles du joueur (pour détecter les actions)
    private PlayerControls controls;
    // Permet de gérer l'action d'interaction
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> interactionAction;
    void Awake()
    {
        // Initialisation des contrôles du joueur
        controls = new PlayerControls();

        // On stocke l'action dans une variable pour pouvoir s'y désabonner
        interactionAction = ctx =>
        {
            if (joueurDansZone)
                ActiverIA();
        };
    }

    void OnEnable()
    {
        // Active les contrôles du joueur
        controls.Enable();
        // Abonnement à l'action d'interaction
        controls.Player.Interact.performed += interactionAction;
    }

    void OnDisable()
    {
        // On se désabonne de l'action d'interaction
        controls.Player.Interact.performed -= interactionAction;
        // Désactive les contrôles du joueur
        controls.Disable();
        // Se désabonner de l'inventaire pour éviter les fuites d'événements
        if (playerInventory != null)
            playerInventory.onResourceChanged -= MettreAJourObjectifUI;
    }

    // Cette fonction est appelée au début du jeu, une seule fois
    void Start()
    {
        // On récupère le composant AudioSource pour pouvoir jouer des sons
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("[AITerminal] Aucun AudioSource trouvé sur ce GameObject.");
        }
        else
        {
            // On s'assure qu'aucun son ne joue automatiquement au lancement
            audioSource.playOnAwake = false;
        }

        // On affiche l'objectif dès le début et on le met à jour
        // Si l'inventaire du joueur n'est pas assigné dans l'inspecteur, on tente de le trouver
        if (playerInventory == null)
        {
            var player = GameObject.FindWithTag("Player");
            if (player != null)
                playerInventory = player.GetComponent<Inventory>();
            if (playerInventory != null)
                Debug.Log("AITerminal: playerInventory trouvé automatiquement.");
            else
                Debug.LogWarning("AITerminal: playerInventory non défini. Assignez l'Inventory du joueur dans l'inspecteur.");
        }

        // S'abonner aux changements d'inventaire pour mettre à jour l'affichage automatiquement
        if (playerInventory != null)
            playerInventory.onResourceChanged += MettreAJourObjectifUI;

        // Met à jour l'affichage initial
        MettreAJourObjectifUI();

        // Ajouter un message d'introduction
        if (messageManager != null)
        {
            messageManager.StartDialogue(dialogue[0]);
        }
    }

    // Fonction qui affiche l'objectif et les ressources actuelles
    private void MettreAJourObjectifUI()
    {
        if (objectifText == null || playerInventory == null) return;

        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        // Format progress bar-like
        string objectifMessage = $"OBJECTIFS:\n";
        objectifMessage += $"Eau: {eau}/{besoinEau} {(eau >= besoinEau ? "✓" : "")}\n";
        objectifMessage += $"Graines: {graines}/{besoinGraines} {(graines >= besoinGraines ? "✓" : "")}\n";
        objectifMessage += $"Engrais: {fertil}/{besoinFertilisant} {(fertil >= besoinFertilisant ? "✓" : "")}";

        // On met à jour le texte de l'objectif en permanence
        // string objectifMessage = $"Objectif : Collecter {besoinEau} eau, {besoinGraines} graines, {besoinFertilisant} engrais.\n" +
        //                          $"Actuellement : Eau ({eau}/{besoinEau}), Graines ({graines}/{besoinGraines}), Engrais ({fertil}/{besoinFertilisant})";

        objectifText.text = objectifMessage;

        // Met également à jour l'objectif principal
        if (objectifText != null)
        {
            int totalRequis = besoinEau + besoinGraines + besoinFertilisant;
            int totalActuel = Mathf.Min(eau, besoinEau) + Mathf.Min(graines, besoinGraines) + Mathf.Min(fertil, besoinFertilisant);
            objectifText.text = $"Revitalisation: {totalActuel}/{totalRequis}";
        }

        // Vérifier si l'objectif est atteint
        if (!objectifAtteint && eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            objectifAtteint = true;
            if (messageManager != null && dialogue != null)
            {
                messageManager.StartDialogue(dialogue[2]);
                messageManager.StartDialogue(dialogue[3]);
            }
        }
    }

    // Ajoutez cette méthode pour être appelée quand une ressource est collectée
    public void OnResourceCollected()
    {
        MettreAJourObjectifUI();
        
        // Vérifie si l'objectif est atteint
        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        if (!objectifAtteint && eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            objectifAtteint = true;
            if (messageManager != null && dialogue != null)
            {
                messageManager.StartDialogue(dialogue[2]);
            }
        }
    }


    // Cette fonction est appelée à chaque image du jeu (60 fois par seconde environ)
    void Update()
    {
        // Si le joueur est proche du terminal et appuie sur la touche E, on tente d'activer l'IA
        if (joueurDansZone && Input.GetKeyDown(KeyCode.E))
        {
            ActiverIA();
        }
    }

    // Cette fonction est appelée quand le joueur utilise le bouton d'interaction (manette ou clavier)
    public void OnInteract(InputValue value)
    {
        // On vérifie que le joueur est bien dans la zone et qu'il vient d'appuyer sur le bouton
        if (joueurDansZone && value.isPressed)
        {
            ActiverIA();
        }
    }

    // Cette fonction tente d'activer l'IA si le joueur a assez de ressources
     // Fonction qui vérifie si le joueur a assez de ressources pour activer les zones 
    void ActiverIA()
    {
        // Inventaire du joueur (pour vérifier les ressources)
        if (playerInventory == null) return;

        // On récupère le nombre de ressources du joueur en temps réel
        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        Debug.Log($"[DEBUG] Inventaire : Eau={eau}, Graines={graines}, Engrais={fertil}");

        // On vérifie si le joueur a toutes les ressources NECESSAIRES
        // On vérifie si l'objectif est atteint
        bool aToutesLesRessources = eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant;

        if (aToutesLesRessources)
        {
            // Le joueur a les ressources nécessaires, on lance le processus

            // C'est ici que le dialogue de succès doit apparaître !
            if (messageManager != null && dialogue != null)
            {
                messageManager.StartDialogue(dialogue[2]);
            }

            // On active toutes les zones à revitaliser
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null) zone.SetActive(true);
            }

            // On joue un son de succès si tout est bien configuré
            if (audioSource != null && iaInteractionSound != null)
            {
                audioSource.PlayOneShot(iaInteractionSound);
                Debug.Log("[AITerminal] Son succès joué.");
            }

            // Optionnel : Désactiver le terminal une fois l'objectif atteint pour éviter des activations répétées
            this.enabled = false;
        }
        else
        {
            // Sinon, on affiche un message d'échec
            // Le joueur n'a pas assez de ressources
            if (messageManager != null && dialogue != null)
            {
                messageManager.StartDialogue(dialogue[1]);
            };

            // Et on joue un son d'échec si tout est bien configuré
            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
                Debug.Log("[AITerminal] Son échec joué.");
            }
        }
    }

    // Appeler cette fonction à chaque fois qu'une ressource est collectée
    // public void AjouterRessource()
    // {
    //     if (playerInventory == null) return;

    //     // Mise à jour de l'affichage de l'objectif
    //     MettreAJourObjectifUI();

    //     // On récupère le nombre de ressources du joueur en temps réel
    //     int eau = playerInventory.GetWaterDropCount();
    //     int graines = playerInventory.GetSeedCount();
    //     int fertil = playerInventory.GetFertilizerCount();

    //     // Si l'objectif n'est pas encore atteint et que les conditions sont remplis
    //     if (!objectifAtteint && eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
    //     {
    //         // Les objectifs sont atteints
    //         objectifAtteint = true;

    //         // On affiche le message d'objectif atteint
    //          messageManager.ShowMessage("[ I.A LOG ] Objectif atteint. Parlez à l'IA pour continuer !", 5.0f);
    //     }
    // }

    // Fonction appelée quand le joueur sort de la zone du terminal
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // On vérifie que le joueur se trouve dans la zone du terminal IA
            joueurDansZone = true;
            // On vérifie les ressources du joueur
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                // string itemName = ConvertItemTypeToName(itemType);
                // inventory.AddItem(itemName);

                // Notifie le terminal IA de la collecte
                AITerminal terminal = FindFirstObjectByType<AITerminal>();
                if (terminal != null)
                {
                    terminal.OnResourceCollected();
                }

                // if (pickupSound != null)
                // {
                //     AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, pickupVolume);
                // }
                // Destroy(gameObject);
            }
        }
    }

    // Fonction appelée quand le joueur sort de la zone du terminal
        void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            // On peut appeler une fonction dans le MessageManager pour cacher le panel.
            if (messageManager != null)
            {
                messageManager.HidePanel();
            }
        }
    }

}
