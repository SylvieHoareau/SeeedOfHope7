using UnityEngine;

public class PitayaPickup : MonoBehaviour
{
    public ParticleSystem pickupEffect; // assigner un autre Particle System pour le burst

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Ajoute vie au joueur
            other.GetComponent<PlayerHealth>().AddHealth(20);

            // Jouer un effet de burst
            if (pickupEffect != null)
            {
                GameObject PickupParticule= Instantiate(pickupEffect, transform.position, Quaternion.identity).gameObject;
                Destroy(PickupParticule, pickupEffect.main.duration);
            }

            Destroy(gameObject); // supprime le pitaya
        }
    }
}
