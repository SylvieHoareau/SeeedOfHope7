using UnityEngine;
using System.Collections;

public class AttackHitbox : MonoBehaviour
{
    [Header("Recul / Dégâts")]
    public float pushDistance = 0.5f;
    public float damageAmount = 0.1f;
    public float pushDuration = 0.1f;

    [Header("Tailles et offsets")]
    public Vector2 verticalSize = new Vector2(0.5f, 1.5f);
    public Vector2 horizontalSize = new Vector2(1.5f, 0.5f);

    public Vector2 offsetRight = new Vector2(1f, 0f);
    public Vector2 offsetLeft  = new Vector2(-1f, 0f);
    public Vector2 offsetUp    = new Vector2(0f, 1f);
    public Vector2 offsetDown  = new Vector2(0f, -1f);

    private BoxCollider2D hitbox;
    private bool isActive = false;

    private void Awake()
    {
        hitbox = GetComponent<BoxCollider2D>();
        hitbox.enabled = false;
    }

    public void Activate(Vector2 direction)
    {
        if (isActive) return;
        isActive = true;

        direction = direction.normalized;

        // Désactive le collider avant de changer taille/offset
        hitbox.enabled = false;

        // Change la taille et l'offset selon direction
        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            hitbox.size = verticalSize;
            hitbox.offset = direction.x > 0 ? offsetRight : offsetLeft;
        }
        else
        {
            hitbox.size = horizontalSize;
            hitbox.offset = direction.y > 0 ? offsetUp : offsetDown;
        }

        // Réactive le collider
        hitbox.enabled = true;

        StartCoroutine(DisableHitboxAfter(pushDuration));
    }

    private IEnumerator DisableHitboxAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        hitbox.enabled = false;
        isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy")) return;

        // Knockback
        Rigidbody2D enemyRb = collision.attachedRigidbody;
        if (enemyRb != null)
        {
            Vector2 knockbackDir = -(collision.transform.position - transform.position).normalized;
            Vector2 targetPos = (Vector2)collision.transform.position + knockbackDir * pushDistance;
            StartCoroutine(PushEnemy(enemyRb, targetPos, pushDuration));
        }

        // Dégâts
        EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
            enemyHealth.Damage(damageAmount);

        // Stun IA
        AiChase enemyAi = collision.GetComponent<AiChase>();
        if (enemyAi != null)
            enemyAi.Stun(pushDuration);
    }

    private IEnumerator PushEnemy(Rigidbody2D enemyRb, Vector2 targetPos, float duration)
    {
        Vector2 startPos = enemyRb.position;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            enemyRb.MovePosition(Vector2.Lerp(startPos, targetPos, elapsed / duration));
            yield return null;
        }
        enemyRb.MovePosition(targetPos);
    }
}
