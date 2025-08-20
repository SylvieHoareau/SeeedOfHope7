using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuSelector : MonoBehaviour
{
    public EventSystem eventSystem;      // référence à l’EventSystem
    public GameObject firstSelected;     // le bouton à sélectionner par défaut (ex: Play)

    void OnEnable()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(firstSelected);
    }
}