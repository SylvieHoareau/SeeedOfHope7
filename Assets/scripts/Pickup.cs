using UnityEngine;

public class Pickup : MonoBehaviour
{
    public enum ItemType
    {
        WaterDrop,
        Seed,
        Fertilizer
    }

    public ItemType itemType = ItemType.WaterDrop;

    [Header("Audio")]
    public AudioClip pickupSound; // À assigner dans l'inspecteur
    [Range(0f, 1f)]
    public float pickupVolume = 1f;

    private AudioSource audioSource;

    void Start()
    {
        // Position Z à 0 pour éviter tout souci de profondeur
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;

        // On vérifie s'il y a un AudioSource, sinon on en ajoute un
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Facultatif : tu peux désactiver spatialBlend pour que le son soit 2D
        audioSource.spatialBlend = 0f; // 0 = son 2D, 1 = son 3D
        audioSource.playOnAwake = false;
    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + Vector3.up * 0.5f, Color.red);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Quelque chose est entrée : " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("C’est le joueur !");
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null)
            {
                string itemName = ConvertItemTypeToName(itemType);
                inventory.AddItem(itemName);

                // Joue le son localement depuis l'objet (non dépendant de la caméra)
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, Camera.main.transform.position, pickupVolume);
                }
                Destroy(gameObject); // Supprime l’objet une fois ramassé
            }
        }
    }

    private string ConvertItemTypeToName(ItemType type)
    {
        switch (type)
        {
            case ItemType.WaterDrop:
                return "Water Drop";
            case ItemType.Seed:
                return "Seed";
            case ItemType.Fertilizer:
                return "Fertilizer";
            default:
                return "Unknown";
        }
    }
}
