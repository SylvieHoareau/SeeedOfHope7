using UnityEngine;
using UnityEngine.EventSystems;

public class OptionPanelSelector : MonoBehaviour
{
    public EventSystem eventSystem;       // référence vers ton EventSystem
    public GameObject firstSelected;      // bouton à sélectionner par défaut

    void OnEnable()
    {
        // On reset la sélection actuelle
        eventSystem.SetSelectedGameObject(null);

        // On force la sélection sur ton bouton du panel
        eventSystem.SetSelectedGameObject(firstSelected);
    }
}