using UnityEngine;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject mainMenuPanel; // Optionnel : si tu veux cacher le menu principal

    void Update()
    {
        // Détecte le bouton B de la manette (bouton 1 sur PS4, bouton 1 sur Xbox)
        if (Input.GetButtonDown("Cancel")) 
        {
            // Vérifie si le panneau d'options est actif
            if (optionsPanel.activeSelf)
            {
                CloseOptions();
            }
        }
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false); // Cacher le menu principal
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true); // Afficher le menu principal
    }
}