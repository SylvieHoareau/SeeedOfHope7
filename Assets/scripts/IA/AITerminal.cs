
// Ce script gère le terminal d'intelligence artificielle (IA) dans le jeu.
// Il permet au joueur d'utiliser des ressources pour revitaliser des zones, avec des sons et des messages d'interface.
// Les commentaires sont adaptés pour être compris par tous, même sans connaissances en programmation !

using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections; // Ajout de la bibliothèque pour les coroutines


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
    public TextMeshProUGUI messageUI;

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

    // --- DEBUT DES MODIFICATIONS
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
    }

    // --- FIN DES MODIFICATIONS


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

        // On affiche un message au joueur dès le début du jeu pour lui indiquer l'objectif.
        // Les variables besoinEau, besoinGraines, et besoinFertilisant sont publiques et peuvent être
        // ajustées dans l'éditeur Unity pour chaque niveau.
        AfficherObjectif();
    }

     // NOUVELLE FONCTION : pour afficher le message de l'objectif initial
    private void AfficherObjectif()
    {
        AfficherMessage($"[ I.A LOG ] Objectif : Collecter {besoinEau} eau, {besoinGraines} graines, et {besoinFertilisant} fertilisants.");
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

     // Ajout d'une fonction centrale pour afficher les messages
    // Cela rend le code plus propre et plus facile à maintenir
    public void AfficherMessage(string message)
    {
        if (messageUI != null)
        {
            messageUI.text = message;
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

        Debug.Log($"[DEBUG] Inventaire : Eau={eau}, Graines={graines}, Fertilisant={fertil}");

        // On vérifie si le joueur a toutes les ressources NECESSAIRES
        if (eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            // Le joueur a les ressources nécessaires, on lance le processus
            // On active toutes les zones à revitaliser
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null)
                    zone.SetActive(true);
            }

            // On affiche le message de succès en utilisant notre nouvelle fonction
            AfficherMessage("[ I.A LOG ] Ressources suffisantes.\nRevitalisation en cours ... trouvez la porte de sortie");

            // On joue un son de succès si tout est bien configuré
            if (audioSource != null && iaInteractionSound != null)
            {
                audioSource.PlayOneShot(iaInteractionSound);
                Debug.Log("[AITerminal] Son succès joué.");
            }
        }
        else
        {
            // Sinon, on affiche un message d'échec
            // On lance la Coroutine pour afficher le message d'erreur temporairement
            StartCoroutine(AfficherMessageTemporaire("[ I.A LOG ] Ressources insuffisantes.\nAnalyse en attente...", 3.0f));

            // Et on joue un son d'échec si tout est bien configuré
            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
                Debug.Log("[AITerminal] Son échec joué.");
            }
        }
    }

      // NOUVELLE FONCTION : Coroutine pour afficher un message temporairement
    IEnumerator AfficherMessageTemporaire(string message, float duree)
    {
        // On affiche le message d'erreur
        AfficherMessage(message);

        // On attend un certain temps
        yield return new WaitForSeconds(duree);

        // Après le temps d'attente, on affiche à nouveau l'objectif initial
        AfficherObjectif();
    }

    // Appeler cette fonction à chaque fois qu'une ressource est collectée
    public void AjouterRessource()
    {
        if (playerInventory == null) return;

        // On récupère le nombre de ressources du joueur en temps réel
        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        // Si l'objectif n'est pas encore atteint et que les conditions sont remplis
        if (!objectifAtteint && eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            // Les objectifs sont atteints
            objectifAtteint = true;

            // On affiche le message d'objectif atteint
            AfficherMessage("[ I.A LOG ] Objectif atteint. Parlez à l'IA pour continuer !");
        }
    }

    // Fonction appelée quand le joueur sort de la zone du terminal
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // On vérifie que le joueur se trouve dans la zone du terminal IA
            joueurDansZone = true;
            // On vérifie les ressources du joueur
            int eau = playerInventory.GetWaterDropCount();
            int graines = playerInventory.GetSeedCount();
            int fertil = playerInventory.GetFertilizerCount();

            if (eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
            {
                AfficherMessage("[ I.A LOG ] Objectif atteint. Appuyer sur la touche A pour continuer.");
            }
            else
            {
                AfficherMessage("[ I.A. LOG] Appuyer sur la touche A pour interagir.");
            }
        }
    }

    // Fonction appelée quand le joueur sort de la zone du terminal
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            AfficherMessage("");
        }
    }

}
