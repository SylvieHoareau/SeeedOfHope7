using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Dictionnaire pour stocker les objets et leur quantité
    [SerializeField]
    private Dictionary<string, int> items = new Dictionary<string, int>();

    // Nombre de gouttes collectées (exposé dans l'inspecteur Unity)
    [SerializeField]
    private int waterDropCount = 0;

    [SerializeField]
    private int seedCount = 0;

    [SerializeField]
    private int fertilizerCount = 0;

    public void AddItem(string itemName)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName]++;
        }
        else
        {
            items[itemName] = 1;
        }

        // Si l'objet est une goutte d'eau, incrémente la variable dédiée
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

        Debug.Log("Objet ajouté à l’inventaire : " + itemName + " (x" + items[itemName] + ")");
    }

    public void ShowInventory()
    {
        Debug.Log("Inventaire :");
        foreach (var kvp in items)
        {
            Debug.Log($"- {kvp.Key} x{kvp.Value}");
        }

        Debug.Log("Gouttes d'eau collectées : " + waterDropCount);
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
