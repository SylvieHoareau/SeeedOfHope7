
// Ce script gère le terminal d'intelligence artificielle (IA) dans le jeu.
// Il permet au joueur d'utiliser des ressources pour revitaliser des zones, avec des sons et des messages d'interface.
// Les commentaires sont adaptés pour être compris par tous, même sans connaissances en programmation !

using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;


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
    public int besoinEau = 1;
    // Nombre de graines nécessaires
    public int besoinGraines = 1;
    // Nombre de fertilisant nécessaires
    public int besoinFertilisant = 1;

    [Header("Audio")]
    // Permet de jouer des sons dans le jeu
    private AudioSource audioSource;
    // Son joué quand l'IA est activée avec succès
    public AudioClip iaInteractionSound;
    // Son joué si le joueur n'a pas assez de ressources
    public AudioClip ressourcesInsuffisantesSound;

    // Indique si le joueur est proche du terminal IA
    private bool joueurDansZone = false;


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
    void ActiverIA()
    {
        // On vérifie que l'inventaire du joueur est bien assigné
        if (playerInventory == null)
        {
            Debug.LogWarning("[AITerminal] Aucun inventaire assigné.");
            return;
        }

        // On récupère le nombre d'unités de chaque ressource dans l'inventaire du joueur
        int eau = playerInventory.GetWaterDropCount();
        int graines = playerInventory.GetSeedCount();
        int fertil = playerInventory.GetFertilizerCount();

        Debug.Log($"[DEBUG] Inventaire : Eau={eau}, Graines={graines}, Fertilisant={fertil}");

        // Si le joueur a assez de chaque ressource...
        if (eau >= besoinEau && graines >= besoinGraines && fertil >= besoinFertilisant)
        {
            // On active toutes les zones à revitaliser (elles deviennent visibles ou accessibles)
            foreach (GameObject zone in zonesARevitaliser)
            {
                if (zone != null)
                    zone.SetActive(true);
            }

            // On affiche un message de succès à l'écran
            messageUI.text = "[ I.A LOG ] Ressources suffisantes.\nRevitalisation en cours ... trouvez la porte de sortie";

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
            messageUI.text = "[ I.A LOG ] Ressources insuffisantes.\nAnalyse en attente...";

            // Et on joue un son d'échec si tout est bien configuré
            if (audioSource != null && ressourcesInsuffisantesSound != null)
            {
                audioSource.PlayOneShot(ressourcesInsuffisantesSound);
                Debug.Log("[AITerminal] Son échec joué.");
            }
        }
    }


    // Cette fonction est appelée quand le joueur entre dans la zone du terminal IA
    void OnTriggerEnter2D(Collider2D other)
    {
        // On vérifie que c'est bien le joueur qui entre dans la zone
        if (other.CompareTag("Player"))
        {
            joueurDansZone = true;
            // On affiche un message pour expliquer comment interagir
            messageUI.text = "[ I.A LOG ] Appuyer sur E (clavier) ou A ou triangle (manette) pour interagir.";
        }
    }

    // Cette fonction est appelée quand le joueur sort de la zone du terminal IA
    void OnTriggerExit2D(Collider2D other)
    {
        // On vérifie que c'est bien le joueur qui sort de la zone
        if (other.CompareTag("Player"))
        {
            joueurDansZone = false;
            // On efface le message d'interaction
            messageUI.text = "";
        }
    }
}
