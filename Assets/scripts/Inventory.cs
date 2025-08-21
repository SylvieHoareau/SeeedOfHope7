using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Cette action permet de prévenir d'autres scripts quand une ressource est ajoutée
    public event System.Action onResourceChanged;

    // Variables pour stocker le nombre de chaque ressource collectée
    [SerializeField] private int waterDropCount = 0; // Eau
    [SerializeField] private int seedCount = 0;      // Graines
    [SerializeField] private int fertilizerCount = 0;// Engrais

    public void AddItem(string itemName)
    {
        // Selon le nom de l'objet, on ajoute 1 à la ressource correspondante
        switch (itemName)
        {
            case "Water Drop":
                waterDropCount++;
                break;
            case "Seed":
                seedCount++;
                break;
            case "Fertilizer":
                fertilizerCount++;
                break;
        }

        // Affiche dans la console ce qui a été ramassé
        Debug.Log($"Objet ajouté à l’inventaire : {itemName}");

        // Préviens les autres scripts qu'une ressource a été ajoutée
        if (onResourceChanged != null)
            onResourceChanged.Invoke();
    }

    public void ShowInventory()
    {
        // Affiche le contenu de l'inventaire dans la console Unity
        Debug.Log($"Eau : {waterDropCount}");
        Debug.Log($"Graines : {seedCount}");
        Debug.Log($"Engrais : {fertilizerCount}");
    }

    // Accesseur public pour d'autres scripts (UI par exemple)
    public int GetWaterDropCount()
    {
        return waterDropCount;
    }

    public int GetSeedCount()
    {
        return seedCount;
    }

    public int GetFertilizerCount()
    {
        return fertilizerCount;
    }
}
