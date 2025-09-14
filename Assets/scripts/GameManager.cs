using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Rendre l'instance unique accessible de partout
    public static GameManager Instance { get; private set; }

    [Header("Configuration des Niveaux")]
    // Liste des ScriptableObjects de niveau à assigner dans l'inspecteur
    public List<LevelData> niveaux;

    // L'index du niveau actuel
    private int niveauActuelIndex = 0;

    void Awake()
    {
        // Implémentation du Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Garde le GameManager entre les scènes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Charge les données du niveau actuel.
    /// </summary>
    public LevelData GetCurrentLevelData()
    {
        if (niveaux == null || niveaux.Count == 0)
        {
            Debug.LogError("GameManager: La liste des niveaux est vide !");
            return null;
        }

        if (niveauActuelIndex >= niveaux.Count)
        {
            Debug.LogWarning("GameManager: Tous les niveaux sont terminés. Le jeu est fini !");
            // Gérer ici la fin du jeu, ex: retour au menu principal
            return null;
        }

        return niveaux[niveauActuelIndex];
    }
    
    /// <summary>
    /// Passe au niveau suivant et le charge.
    /// </summary>
    public void LoadNextLevel()
    {
        niveauActuelIndex++;
        // Ici, vous pouvez ajouter la logique pour charger la prochaine scène
        // par exemple: SceneManager.LoadScene("NomDeLaProchaineScene");
    }
}