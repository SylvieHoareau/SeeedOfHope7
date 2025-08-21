using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Important pour utiliser les Queues (files d'attente)

public class MessageManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private Coroutine currentCoroutine; // Pour éviter plusieurs coroutines

    private Coroutine hideCoroutine;
    // File d'attente pour les phrases
    // private Queue<string> sentencesQueue;
    // Pour savoir si un dialogue est en cours
    // private bool isDialogueActive = false;

    void Awake()
    {
        // Par sécutité : on cache le panel au lancement
        if (messagePanel != null)
            messagePanel.SetActive(false);
    }

    /// <summary>
    ///    Affiche un message temporaire pendant X secondes puis le masque 
    /// </summary>

    public void ShowMessage(string message, float duration)
    {
        if (messagePanel == null || messageText == null)
        {
            Debug.LogWarning("[MessageManager] Références UI manquantes");
            return;
        }

        // Si une autre coroutine est en cours, on la stoppe
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        messagePanel.SetActive(true);
        messageText.text = message;

        // Lance une coroutine pour masquer après X secondes
        currentCoroutine = StartCoroutine(HideAfterSeconds(duration));
    }

    /// <summary>
    /// Démarre un dialogue basé sur un ScriptableObject Dialogue
    /// (Ici, on affiche seulement la première phrase pour simplifier)
    /// </summary>
    public void StartDialogue(Dialogue dialogue)
    {
        if (messagePanel == null || messageText == null)
        {
            Debug.LogWarning("[Message Manager] Références UI manquantes ! ");
            return;
        }

        if (dialogue == null || dialogue.sentences.Length == 0)
        {
            Debug.LogWarning("[MessageManager] Dialogue vide ou non assigné");
            return;
        }

        // Si une autre coroutine est en cours, on la stoppe
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        messagePanel.SetActive(true);
        messageText.text = dialogue.sentences[0]; // Pour l’instant une phrase simple
    }

    /// <summary>
    /// Masque immédiatement le panneau de message.
    /// </summary>
    public void HidePanel()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false);

        if (messageText != null)
            messageText.text = "";

        currentCoroutine = null;
    }

    /// <summary>
    /// Coroutine pour masquer après X secondes.
    /// </summary>
    private IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        HidePanel();
    }
    }

    // void Start()
    // {
    //     // On initialise la file d'attente
    //     sentencesQueue = new Queue<string>();
    // }

    // void Update()
    // {
    //     // Si un dialogue est actif et que le joueur appuie sur la touche d'interaction
    //     // (ici 'A' sur le clavier ou le bouton Sud de la manette)
    //     if (isDialogueActive && Input.GetButtonDown("Fire1")) // "Fire1" est souvent mappé au clic gauche ou à une touche d'action
    //     {
    //         DisplayNextSentence();
    //     }
    // }

    