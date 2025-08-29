using UnityEngine;
using System.Collections.Generic;

public class AttackHitbox : MonoBehaviour
{
    [Header("Paramètres de l'attaque")]
    public float damage = 1f;
    public float knockbackForce = 5f; // Force du knockback
    public float stunDuration = 0.5f;
    public float hitboxOffset = 1f; // Distance de la hitbox par rapport au joueur
    public Vector2 hitboxSize = new Vector2(1f, 1f); // Taille de la hitbox

    [Header("Debug")]
    public bool showDebugGizmos = true;
    public bool showDebugLogs = true;

    private BoxCollider2D hitboxCollider;

    // Liste pour éviter de toucher le même ennemi plusieurs fois
    private List<GameObject> hitEnemies = new List<GameObject>(); // Évite les hits multiples
    private Vector2 currentDirection;

    [SerializeField]
    public int attackDamage = 5;


    void OnEnable()
    {
        // Réinitialise la liste quand la hitbox est réactivée
        hitEnemies.Clear();
        if (showDebugLogs)
            Debug.Log("AttackHitbox activée - Liste des ennemis réinitialisée");
    }


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

     private void OnTriggerEnter2D(Collider2D collision)
    {
        
        // ⚔️  Vérifie si la collision est avec un ennemi
        // if (collision.CompareTag("Enemy"))
        // {
        //     Debug.Log("Hit " + collision.name);
        //     // 🎯 Cherche le composant EnemyHealth sur l'ennemi
        //     EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        //     if (enemyHealth != null)
        //     {
        //         // 🔥 Inflige des dégâts à l'ennemi
        //         enemyHealth.TakeDamage(attackDamage);
        //     }
        // }

        if (collision.CompareTag("Enemy") && !hitEnemies.Contains(collision.gameObject))
        {
            // Ajoute l'ennemi à la liste pour éviter les hits multiples
            hitEnemies.Add(collision.gameObject);

            if (showDebugLogs)
                Debug.Log($"Ennemi touché: {collision.name}");

            // 1. APPLIQUER LES DÉGÂTS (LE PLUS IMPORTANT!)
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                if (showDebugLogs)
                    Debug.Log($"Dégâts appliqués: {damage} à {collision.name}");
            }
            else
            {
                Debug.LogWarning($"EnemyHealth introuvable sur {collision.name}!");
            }

            // 2. APPLIQUER LE STUN
            AiChase aiChase = collision.GetComponent<AiChase>();
            if (aiChase != null)
            {
                aiChase.Stun(stunDuration);
                if (showDebugLogs)
                    Debug.Log($"Stun appliqué: {stunDuration}s à {collision.name}");
            }

            // 3. APPLIQUER LE KNOCKBACK
            Rigidbody2D enemyRb = collision.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                // Calcul la direction du knockback
                Vector2 knockbackDir = (collision.transform.position - transform.position).normalized;

                // S'assure qu'il y a une direction valide
                if (knockbackDir.sqrMagnitude < 0.1f)
                {
                    knockbackDir = Vector2.up; // Direction par défaut
                }

                // Applique la force de knockback
                enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);

                if (showDebugLogs)
                    Debug.Log($"Knockback appliqué: {knockbackDir} * {knockbackForce} à {collision.name}");
            }
        }
    }

    // Méthode publique pour réinitialiser la hitbox
    public void ResetHitbox()
    {
        hitEnemies.Clear();
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