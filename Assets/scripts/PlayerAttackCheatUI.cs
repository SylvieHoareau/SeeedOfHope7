using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerAttackCheatUI : MonoBehaviour
{
    [Header("Cheat Settings")]
    public float normalDamage = 1f;
    public float boostedDamage = 5f;
    
    [Header("UI Elements")]
    public TMP_Text cheatText;       // Texte temporaire
    public GameObject fireIcon;      // Icône feu affichée quand boost actif

    [Header("References")]
    public PlayerAttack playerAttack; // Référence au script de dégâts du joueur

    private Coroutine cheatMessageCoroutine;
    private string cheatSequence = "ABXY"; // Séquence de touches
    private string currentInput = "";

    private void Start()
    {
        if (cheatText != null) cheatText.gameObject.SetActive(false);
        if (fireIcon != null) fireIcon.SetActive(false);

        if (playerAttack != null)
            playerAttack.damageMultiplier = normalDamage;
    }

    private void Update()
    {
        // Clavier
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame) AddInput("A");
            if (Keyboard.current.bKey.wasPressedThisFrame) AddInput("B");
            if (Keyboard.current.xKey.wasPressedThisFrame) AddInput("X");
            if (Keyboard.current.yKey.wasPressedThisFrame) AddInput("Y");
        }

        // Manette
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame) AddInput("A"); // A
            if (Gamepad.current.buttonEast.wasPressedThisFrame) AddInput("B");  // B
            if (Gamepad.current.buttonWest.wasPressedThisFrame) AddInput("X");  // X
            if (Gamepad.current.buttonNorth.wasPressedThisFrame) AddInput("Y"); // Y
        }
    }

    private void AddInput(string key)
    {
        currentInput += key;

        // Limiter la taille
        if (currentInput.Length > cheatSequence.Length)
            currentInput = currentInput.Substring(currentInput.Length - cheatSequence.Length);

        // Vérifier la séquence
        if (currentInput == cheatSequence)
        {
            ToggleCheat();
            currentInput = ""; // reset
        }
    }

    private void ToggleCheat()
    {
        if (playerAttack == null) return;

        if (playerAttack.damageMultiplier == normalDamage)
        {
            playerAttack.damageMultiplier = boostedDamage;
            ShowCheatMessage("Boost d'attaque ACTIVÉ (x5)");
            if (fireIcon != null) fireIcon.SetActive(true);
        }
        else
        {
            playerAttack.damageMultiplier = normalDamage;
            ShowCheatMessage("Boost désactivé");
            if (fireIcon != null) fireIcon.SetActive(false);
        }
    }

    private void ShowCheatMessage(string message)
    {
        if (cheatMessageCoroutine != null) StopCoroutine(cheatMessageCoroutine);
        cheatMessageCoroutine = StartCoroutine(CheatMessageRoutine(message));
    }

    private System.Collections.IEnumerator CheatMessageRoutine(string message)
    {
        if (cheatText == null) yield break;

        cheatText.text = message;
        cheatText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        cheatText.gameObject.SetActive(false);
    }
}
