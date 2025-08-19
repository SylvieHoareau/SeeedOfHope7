using UnityEngine;
using UnityEngine.UI; // si tu veux modifier du texte
using UnityEngine.InputSystem; // si tu utilises le nouveau Input System

public class MovementHint : MonoBehaviour
{
    public GameObject hintUI; // le panneau ou texte d'aide

    private PlayerControls controls;
    private bool hasMoved = false;

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    void Update()
    {
        if (hasMoved) return;

        // Vérifie si une touche ou le stick gauche bouge
        Vector2 moveInput = controls.Player.Move.ReadValue<Vector2>();

        if (moveInput.magnitude > 0.1f) // joueur a bougé
        {
            hasMoved = true;
            hintUI.SetActive(false); // cache le panneau
        }
    }
}
