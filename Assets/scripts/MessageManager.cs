using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // Important pour utiliser les Queues (files d'attente)

public class MessageManager : MonoBehaviour
{
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    // Pour afficher le nom qui parle
    // public TextMeshProUGUI speakerNameText; 
    private Coroutine hideCoroutine;
    // File d'attente pour les phrases
    private Queue<string> sentencesQueue;
    // Pour savoir si un dialogue est en cours
    private bool isDialogueActive = false;

    void Start()
    {
        // On initialise la file d'attente
        sentencesQueue = new Queue<string>();
    }

    void Update()
    {
        // Si un dialogue est actif et que le joueur appuie sur la touche d'interaction
        // (ici 'A' sur le clavier ou le bouton Sud de la manette)
        if (isDialogueActive && Input.GetButtonDown("Fire1")) // "Fire1" est souvent mappé au clic gauche ou à une touche d'action
        {
            DisplayNextSentence();
        }
    }

    // Fonction pour afficher le message avec un délai
    public void ShowMessage(string message, float delay = 15.0f)
    {
        // Activer le panel
        messagePanel.SetActive(true);
        // Définit le texte
        messageText.text = message;

        // Si une coroutine est dèjà en cours, on l'arrête pour éviter
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        // Lance la coroutine qui va cacher le panel après 15 secondes
        hideCoroutine = StartCoroutine(HidePanelAfterDelay(15f));
    }

    // --- Fonction pour le système de dialogue ---
    public void StartDialogue(Dialogue dialogue)
    {
        isDialogueActive = true;
        messagePanel.SetActive(true);

        // Affiche le nom du locuteur
        // if (speakerNameText != null) speakerNameText.text = dialogue.speakerName;

        // On vide la file d'attente au cas où
        sentencesQueue.Clear();

        // On remplit la file d'attente avec les phrases du dialogue
        foreach (string sentence in dialogue.sentences)
        {
            sentencesQueue.Enqueue(sentence);
        }

        // On affiche la première phrase
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        // S'il n'y a plus de phrases, on termine le dialogue
        if (sentencesQueue.Count == 0)
        {
            EndDialogue();
            return;
        }

        // On récupère la phrase suivante et on l'affiche
        string sentence = sentencesQueue.Dequeue();
        // Ajouter un effet de machine à écrire
        StopAllCoroutines(); // Arrête l'animation de texte précédent
        StartCoroutine(TypeSentence(sentence));

    }

    // Effet "machine à écrire" pour afficher le texte lettre par lettre
    IEnumerator TypeSentence(string sentence)
    {
        messageText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        messagePanel.SetActive(false);
    }

    public void HidePanel()
    {
        // On s'assure qu'on n'arrête pas une coroutine qui n'existe pas
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
        // On désactive le panel pour le cacher
        messagePanel.SetActive(false);
    }


    // Coroutine pour cacher le panel après un délai
    private IEnumerator HidePanelAfterDelay(float delay)
    {
        // Attend le nombre de secondes spécifié
        yield return new WaitForSeconds(delay);
        // Désactive le panel
        messagePanel.SetActive(false);
        // Réinitialise la coroutine
        hideCoroutine = null;
    }
}
