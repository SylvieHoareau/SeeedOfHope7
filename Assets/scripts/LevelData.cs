using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    [Header("Objectifs de ressources pour ce niveau")]
    // Utilisez une liste de classes sérialisables pour définir les objectifs
    public List<ResourceGoal> objectifs;

    [Header("Dialogues spécifiques au niveau")]
    // Chaque niveau peut avoir ses propres dialogues
    public Dialogue dialogueBienvenue;
    public Dialogue dialogueObjectifAtteint;
    public Dialogue dialogueObjectifEchec;
    public Dialogue dialogueObjectifDejaAtteint;
}

// Classe pour définir les objectifs de ressources de manière claire
[System.Serializable]
public class ResourceGoal
{
    public ResourceType type;
    public int amount;
    // Ajout d'une variable pour l'icône de la ressource
    public Sprite icon; 
}
