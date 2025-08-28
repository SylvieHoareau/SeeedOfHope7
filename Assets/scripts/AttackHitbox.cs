using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    [Header("Paramètres de l'attaque")]
    public float damage = 1f;
    public float stunDuration = 0.5f;
    public float hitboxOffset = 1f; // Distance de la hitbox par rapport au joueur
    public Vector2 hitboxSize = new Vector2(1f, 1f); // Taille de la hitbox

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public bool showDebugLogs = true;

    private BoxCollider2D hitboxCollider;
    private List<GameObject> hitEnemies = new List<GameObject>(); // Évite les hits multiples
    private Vector2 currentDirection;

    void Awake()
    {
        hitboxCollider = GetComponent<BoxCollider2D>();
        if (hitboxCollider == null)
        {
            hitboxCollider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Configuration du collider
        hitboxCollider.isTrigger = true;
        hitboxCollider.size = hitboxSize;
        
        // Désactiver par défaut
        gameObject.SetActive(false);
    }

    public void Activate(Vector2 direction)
    {
        currentDirection = direction.normalized;
        hitEnemies.Clear(); // Reset la liste des ennemis touchés
        
        // Positionner la hitbox dans la direction d'attaque
        Vector3 playerPos = transform.parent.position;
        Vector3 hitboxPos = playerPos + (Vector3)(currentDirection * hitboxOffset);
        transform.position = hitboxPos;
        
        // Ajuster la rotation de la hitbox si nécessaire
        float angle = Mathf.Atan2(currentDirection.y, currentDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        if (showDebugLogs)
        {
            Debug.Log($"Hitbox activée à la position {hitboxPos} avec direction {currentDirection}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Vérifier si c'est un ennemi et s'il n'a pas déjà été touché
        if (other.CompareTag("Enemy") && !hitEnemies.Contains(other.gameObject))
        {
            hitEnemies.Add(other.gameObject);
            
            if (showDebugLogs)
            {
                Debug.Log($"Ennemi touché: {other.name}");
            }
            
            // Appliquer les dégâts
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.Damage(damage);
            }
            
            // Appliquer le stun
            AiChase aiChase = other.GetComponent<AiChase>();
            if (aiChase != null)
            {
                aiChase.Stun(stunDuration);
            }
            
            // Effet de knockback optionnel
            Rigidbody2D enemyRb = other.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                Vector2 knockbackForce = currentDirection * 5f; // Ajustez la force selon vos besoins
                enemyRb.AddForce(knockbackForce, ForceMode2D.Impulse);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (showDebugGizmos)
        {
            // Dessiner la hitbox en rouge quand elle est active
            if (gameObject.activeInHierarchy)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(transform.position, hitboxSize);
            }
            else
            {
                // Dessiner la hitbox en jaune quand elle est inactive (pour le setup)
                if (transform.parent != null)
                {
                    Gizmos.color = Color.yellow;
                    Vector3 previewPos = transform.parent.position + (Vector3)(Vector2.down * hitboxOffset);
                    Gizmos.DrawWireCube(previewPos, hitboxSize);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (showDebugGizmos && transform.parent != null)
        {
            // Dessiner la portée d'attaque dans toutes les directions
            Gizmos.color = Color.cyan;
            Vector3 playerPos = transform.parent.position;
            
            // Dessiner les 4 directions principales
            Vector2[] directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
            foreach (Vector2 dir in directions)
            {
                Vector3 hitboxPos = playerPos + (Vector3)(dir * hitboxOffset);
                Gizmos.DrawWireCube(hitboxPos, hitboxSize);
            }
        }
    }
}