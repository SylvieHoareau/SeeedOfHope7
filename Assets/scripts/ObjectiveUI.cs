using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ObjectiveUI : MonoBehaviour
{
    [System.Serializable]
    public class ObjectifParNiveau
    {
        public string sceneName;
        public int eau;
        public int graines;
        public int fertilisant;
    }
    public TextMeshProUGUI objectifText;
    public ObjectifParNiveau[] objectifs;

    private AITerminal terminal;

    void Start()
    {
        terminal = FindObjectOfType<AITerminal>();

        string currentScene = SceneManager.GetActiveScene().name;

        foreach (var obj in objectifs)
        {
            if (obj.sceneName == currentScene)
            {
                if (terminal != null)
                {
                    terminal.besoinEau = obj.eau;
                    terminal.besoinGraines = obj.graines;
                    terminal.besoinFertilisant = obj.fertilisant;
                }

                if (objectifText != null)
                {
                    objectifText.text = $"Objectif : Collecte {obj.eau} eau, {obj.graines} graines, {obj.fertilisant} engrais";
                }

                return;
            }
        }

        Debug.LogWarning("Aucun objectif trouvé pour la scène " + currentScene);
    }

    public void AfficherObjectifAtteint()
    {
        if (objectifText != null)
            objectifText.text = "Objectif atteint - Parlez à l'IA (touche A ou E)";
    }
}
