using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    // Durée d'amortissement
    public float timeOffset = 0.2f;
    // Décalage caméra
    public Vector3 posOffset;
    public Vector3 velocity = Vector3.zero;

    void Start()
    {
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }
    }
    void Update()
    {
        if (player == null) return;

        Vector3 targetPos = player.transform.position + posOffset;
        // garder le z de la caméra si on veut éviter qu'elle suive en profondeur
        targetPos.z = transform.position.z; 

        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref velocity, timeOffset);
    }
}
