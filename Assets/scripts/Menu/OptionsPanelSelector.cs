using UnityEngine;
using UnityEngine.EventSystems;

public class OptionPanelSelector : MonoBehaviour
{
    public EventSystem eventSystem;
    public GameObject firstSelected;  // bouton volume ou retour
    public GameObject backButton;     // la croix "Quitter"

    void OnEnable()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(firstSelected);
    }

    void Update()
    {
        // Si j’appuie sur "Cancel" (B sur Xbox, rond sur PS, ou Esc clavier)
        if (Input.GetButtonDown("Cancel"))
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(backButton);
        }
    }
}