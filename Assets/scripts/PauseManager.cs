using UnityEngine;
using UnityEngine.EventSystems;
// using UnityEngine.InputSystem; // You need this library for the new Input System

public class PauseManager : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;

    [SerializeField] private GameObject firstSelectedButton;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.JoystickButton7)) {
            if (pauseMenu != null)
            {
                if (pauseMenu.IsPaused)
                    pauseMenu.Resume();
                else
                    pauseMenu.Pause();
            }
            else
            {
                pauseMenu.Pause();

                // Sléectionner le bouton manuellement pour navigation à la manette
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton);
            }
        }
        
    }

    // This method will be called automatically by the Input System when the "Pause" action is performed.
    // public void OnPause(InputValue value)
    // {
    //     // We check if the button was just pressed
    //     if (value.isPressed)
    //     {
    //         if (pauseMenu != null)
    //         {
    //             if (pauseMenu.IsPaused)
    //                 pauseMenu.Resume();
    //             else
    //                 pauseMenu.Pause();
    //         }
    //     }
    // }
}
