using UnityEngine;
using TMPro;

public class ResourceUI : MonoBehaviour
{
    public Inventory inventory; // à lier dans l'inspecteur
    public TextMeshProUGUI waterText;
    public TextMeshProUGUI seedText;
    public TextMeshProUGUI fertilizerText;


    void Update()
    {
        if (inventory == null) return;

        if (inventory != null && waterText != null)
        {
            waterText.text = "Eau : " + inventory.GetWaterDropCount();
        }

        if (inventory != null && seedText != null)
        {
            seedText.text = "Graines : " + inventory.GetSeedCount();
        }
        
         if (inventory != null && fertilizerText != null)
        {
            fertilizerText.text = "Engrais bio : " + inventory.GetFertilizerCount();
        }
    }
}
