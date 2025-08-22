using TMPro;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private Coroutine currentTypingCoroutine;
    private Queue<string> sentences;

    // Pour savoir si un dialogue est en cours
    public bool isDialogueActive { get; private set; } = false;

    void Awake()
    {
        // On cache le panel au lancement pour qu'il ne soit pas visible
        if (messagePanel != null)
            messagePanel.SetActive(false);
        
        // On initialise la file d'attente pour les phrases
        sentences = new Queue<string>();
    }

    /// <summary>
    /// Affiche un message simple et temporaire pendant une durée donnée.
    /// Idéal pour les messages d'erreur, de succès ou d'objectif.
    /// </summary>
    public void ShowMessage(string message, float duration = 3f)
    {
        isDialogueActive = false;
        
        if (currentTypingCoroutine != null) 
            StopCoroutine(currentTypingCoroutine);

        if (messagePanel != null)
            messagePanel.SetActive(true);
        
        if (messageText != null)
        {
            messageText.text = message;
        }

        StartCoroutine(HideAfterSeconds(duration));
    }

    /// <summary>
    /// Commence une nouvelle conversation avec plusieurs phrases.
    /// </summary>
    public void StartConversation(Dialogue dialogue)
    {
        isDialogueActive = true;
        
        if (currentTypingCoroutine != null) 
            StopCoroutine(currentTypingCoroutine);
            
        sentences.Clear(); 

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

        if (messagePanel != null)
            messagePanel.SetActive(true);
        
        DisplayNextSentence();
    }

    /// <summary>
    /// Affiche la prochaine phrase de la conversation.
    /// </summary>
    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        
        if (currentTypingCoroutine != null)
            StopCoroutine(currentTypingCoroutine);
            
        currentTypingCoroutine = StartCoroutine(TypeSentence(sentence));
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
        
        isDialogueActive = false;
        
        if (currentTypingCoroutine != null) 
            StopCoroutine(currentTypingCoroutine);
    }

    /// <summary>
    /// Met fin au dialogue et masque le panneau.
    /// </summary>
    void EndDialogue()
    {
        HidePanel();
    }

    /// <summary>
    /// Coroutine pour taper le texte lettre par lettre.
    /// </summary>
    IEnumerator TypeSentence(string sentence)
    {
        messageText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            messageText.text += letter;
            yield return new WaitForSeconds(0.05f); // Vitesse d'écriture
        }
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