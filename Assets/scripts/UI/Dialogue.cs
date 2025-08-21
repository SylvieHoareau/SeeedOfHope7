// Ce script n'est pas attaché à un objet, il sert de "modèle" pour organiser les dialogues.
using UnityEngine;

[System.Serializable]
public class Dialogue
{
    // Tableau pour écrire toutes les phrases du dialogue dans l'inspecteur Unity
    [TextArea(3, 10)] // Permet d'avoir une zone de texte plus grande dans l'inspecteur
    public string[] sentences;
}