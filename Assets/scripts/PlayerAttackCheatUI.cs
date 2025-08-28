using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

public class PlayerAttackCheatUI : MonoBehaviour
{
    [Header("Cheat Settings")]
    public float normalDamage = 1f;
    public float boostedDamage = 5f;
    public float damageMultiplier = 1f;

    [Header("UI Elements")]
    public TMP_Text cheatText;       // Texte qui s’affiche temporairement
    public GameObject fireIcon;      // Icône feu affichée quand boost actif

    private Coroutine cheatMessageCoroutine;
    private string cheatSequence = "ABXY"; // exemple pour manette
    private string currentInput = "";

    private void Start()
    {
        if (cheatText != null)
            cheatText.gameObject.SetActive(false);

        if (fireIcon != null)
            fireIcon.SetActive(false);

        damageMultiplier = normalDamage;
    }

    private void Update()
    {
        // On capte les touches (clavier + manette)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.wasPressedThisFrame) AddInput("A");
            if (Keyboard.current.bKey.wasPressedThisFrame) AddInput("B");
            if (Keyboard.current.xKey.wasPressedThisFrame) AddInput("X");
            if (Keyboard.current.yKey.wasPressedThisFrame) AddInput("Y");
        }

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

        // Si la séquence devient trop longue, on coupe
        if (currentInput.Length > cheatSequence.Length)
            currentInput = currentInput.Substring(currentInput.Length - cheatSequence.Length);

        // Vérifie si le cheat code correspond
        if (currentInput == cheatSequence)
        {
            ToggleCheat();
            currentInput = ""; // reset
        }
    }

    private void ToggleCheat()
    {
        if (damageMultiplier == normalDamage)
        {
            damageMultiplier = boostedDamage;
            ShowCheatMessage("Boost d'attaque ACTIVÉ (x5)");
            if (fireIcon != null) fireIcon.SetActive(true);
        }
        else
        {
            damageMultiplier = normalDamage;
            ShowCheatMessage("Boost désactivé");
            if (fireIcon != null) fireIcon.SetActive(false);
        }
    }

    private void ShowCheatMessage(string message)
    {
        if (cheatMessageCoroutine != null)
            StopCoroutine(cheatMessageCoroutine);

        cheatMessageCoroutine = StartCoroutine(CheatMessageRoutine(message));
    }

    private System.Collections.IEnumerator CheatMessageRoutine(string message)
    {
        cheatText.text = message;
        cheatText.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);

        cheatText.gameObject.SetActive(false);
    }
}
