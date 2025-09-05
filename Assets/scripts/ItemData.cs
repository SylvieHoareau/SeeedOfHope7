using UnityEngine;

// Le "CreateAssetMenu" permet de créer facilement un ItemData depuis le menu d'Unity
[CreateAssetMenu(fileName = "ItemData", menuName = "Scriptable Objects/ItemData")]
public class ItemData : ScriptableObject
{
    // Le nom de l'objet, comme "Water Drop"
    public string displayName;

    // Le sprite qui sera affiché dans l'inventaire
    public Sprite icon;
}
