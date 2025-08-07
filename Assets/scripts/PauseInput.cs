using UnityEngine;
using UnityEngine.InputSystem;

public class PauseInput : MonoBehaviour
{
    // Référence à ton script PauseMenu, pour pouvoir l'appeler.
    public PauseMenu pauseMenu;

    // Cette fonction est appelée automatiquement par l'Input System
    // quand le joueur appuie sur le bouton "Pause" (lié au bouton Start de la manette).
    public void OnPause(InputValue value)
    {
        // On vérifie que le bouton a bien été pressé.
        if (value.isPressed && pauseMenu != null)
        {
            // Si le jeu est en pause, on le reprend.
            if (pauseMenu.IsPaused)
            {
                pauseMenu.Resume();
            }
            // Sinon, on met le jeu en pause.
            else
            {
                pauseMenu.Pause();
            }
        }
    }
}