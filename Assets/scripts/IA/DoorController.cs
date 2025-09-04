using UnityEngine;
using UnityEngine.SceneManagement;

// Simple door controller: when opened it allows the player to enter and loads a scene.
// Attach this to the door GameObject. Set the door GameObject inactive in the scene
// (or keep it active but locked). When AITerminal opens the door it will call OpenDoor().
public class DoorController : MonoBehaviour
{
    [Tooltip("Scene name to load when the player enters the door.")]
    public string sceneToLoad = "Level2";

    [Tooltip("If true the door blocks passage until OpenDoor() is called.")]
    public bool isLocked = true;

    // [Tooltip("Optional animator to play an open animation.")]
    // public Animator animator;

    // Collider used to detect player entering the door. Prefer a trigger collider.
    public Collider2D doorTriggerCollider;

    private void Awake()
    {
        if (doorTriggerCollider == null)
            doorTriggerCollider = GetComponent<Collider2D>();

        // ensure trigger collider is enabled if present
        if (doorTriggerCollider != null)
            doorTriggerCollider.isTrigger = true;
    }

    // Called by other scripts (ex: AITerminal) to open the door.
    public void OpenDoor()
    {
        isLocked = false;

        // play open animation if available
        // if (animator != null)
        // {
        //     animator.SetTrigger("Open");
        // }

        // Make sure the collider is enabled so OnTriggerEnter2D will fire
        if (doorTriggerCollider != null)
            doorTriggerCollider.enabled = true;

        // Optionally make the door visible if it was hidden
        gameObject.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isLocked) return;

        if (other.CompareTag("Player"))
        {
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                // Make sure the target scene is added to Build Settings
                SceneManager.LoadScene(sceneToLoad);
            }
        }
    }
}
