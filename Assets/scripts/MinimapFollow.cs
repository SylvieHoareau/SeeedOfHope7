using UnityEngine;

public class MinimapFollow : MonoBehaviour
{
    public GameObject player;
    public float height = 15f;

    void LateUpdate()
    {
        Vector3 newPosition = player.transform.position;
        newPosition.y = height;
        transform.position = newPosition;
    }
}