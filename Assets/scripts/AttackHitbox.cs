using UnityEngine;
using System.Collections;

// Ce script gère la "zone de frappe" d'une attaque, appelée "hitbox"
// C'est ce qui détecte quand une attaque touche un ennemi
public class AttackHitbox : MonoBehaviour
{
    // --- REGLAGES DU JEU --
    // Ces variables peuvent être modifiées dans l'inspecteur pour équilibrer le jeu
    [Header("Recul / Dégâts")]
    // A quelle distance l'ennemi est repoussé
    public float pushDistance = 0.5f;
    // Montant des dégâts infligés
    public float damageAmount = 0.1f;
    // Combien de temps l'ennemi est repoussé
    public float pushDuration = 0.1f;

    [Header("Tailles et offsets")]
    // Taille de la hitbox pour une attaque verticale (haut/bas)
    public Vector2 verticalSize = new Vector2(0.5f, 1.5f);
    // Taille de la hitbox pour une attaque horizontal (gauche/droite)
    public Vector2 horizontalSize = new Vector2(1.5f, 0.5f);

    // Position de la hitbox par rapport au joueur
    public Vector2 offsetRight = new Vector2(1f, 0f);
    public Vector2 offsetLeft = new Vector2(-1f, 0f);
    public Vector2 offsetUp = new Vector2(0f, 1f);
    public Vector2 offsetDown = new Vector2(0f, -1f);

    // --- ELEMENTS TECHNIQUES ---
    // Le composant Unity qui crée la zone de détection
    private BoxCollider2D hitbox;
    // Pour savoir si la hitbox est dèjà en train d'être utilisée 
    private bool isActive = false;

    // "Awake" est appelé lorsque le script est chargé
    private void Awake()
    {
        // On récupère le composant BoxCollider2D pour pouvoir le contrôler
        hitbox = GetComponent<BoxCollider2D>();
        // On s'assure que la hitbox est désactivé au démarrage du jeu
        hitbox.enabled = false;
    }

    // --- L'ATTAQUE ELLE MEME ---

    // Cette fonction est appelée par un autre script
    // pour "lancer" l'attaque dans certaine direction
    public void Activate(Vector2 direction)
    {
        // Si une attaque est déjà en cours, on ne fait rien
        if (isActive) return;

        // On active la hitbox et on la place dans la bonne direction
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            // Si le mouvement est plus horizontal (gauche/droite)
            hitbox.size = horizontalSize;
            hitbox.offset = direction.x > 0 ? offsetRight : offsetLeft;
        }
        else
        {   
            // Si le mouvement est plus vertical (haut/bas)
            hitbox.size = verticalSize  ;
            hitbox.offset = direction.y > 0 ? offsetUp : offsetDown;
        }

        // On lance une routine pour désactiver la hitbox après un court instant
        // Cela crée l'effet d'un "coup rapide"
        StartCoroutine(DisableHitboxAfter(pushDuration));
    }

    // Gère l'activation et la désactivation de la hitbox après la durée de l'attaque
    private IEnumerator ActivateAndDisable()
    {
        // Indique que la hitbox est active
        isActive = true;
        // Active la zone de détection
        hitbox.enabled = true;

        // On attend le temps de l'attaque
        yield return new WaitForSeconds(pushDuration); // pushDuration est utilisé comme durée d'activité

        // Désactive la zone de détection
        hitbox.enabled = false;
        // Indique que la hitbox n'est plus active
        isActive = false;
    }


    // La fonction "OnTriggerEnter2D" est appelée automatiquement par Unity
    // dès que la hitbox (le "Collider") entre en collision avec un autre objet.
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // On vérifie si l'objet touché est bien un ennemi
        if (!collision.CompareTag("Enemy")) return;

        // --- LES EFFETS DE L'ENNEMI ---

        // Knockback (recul de l'ennemi)
        Rigidbody2D enemyRb = collision.attachedRigidbody;
        if (enemyRb != null)
        {
            // On calcule la direction dans laquelle l'ennemi doit être repoussé
            Vector2 knockbackDir = -(collision.transform.position - transform.position).normalized;
            // On détermine la position finale où l'ennemi va être poussé
            Vector2 targetPos = (Vector2)collision.transform.position + knockbackDir * pushDistance;
            // On lance la routine pour pousser l'ennemi
            StartCoroutine(PushEnemy(enemyRb, targetPos, pushDuration));
        }

        // Gérer les dégâts
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.Damage(damageAmount);

        // Gérer l'etourdissement de l'IA
        AiChase enemyAi = collision.GetComponent<AiChase>();
        if (enemyAi != null)
            enemyAi.Stun(pushDuration);
    }

    // Cette routine gère le mouvement de l'ennemi pour le repousser en douceur
    private IEnumerator PushEnemy(Rigidbody2D enemyRb, Vector2 targetPos, float duration)
    {
        Vector2 startPos = enemyRb.position;
        float elapsed = 0f;

        // On déplace l'ennemi petit à petit jusqu'à la position cible
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // On déplace l'ennemi en le "malangeant" entre les positions de départ et la position cible
            enemyRb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / duration));
            yield return null;
        }
        // Pour s'assurer que l'ennemi arrive bien à la position finale
        enemyRb.MovePosition(targetPos);
    }
}
