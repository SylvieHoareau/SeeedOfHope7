using UnityEngine;
using System.Collections;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 2f;

    // Distances
    public float stickDistance = 0.5f;   // Distance minimale avant d'arrêter l'ennemi
    public float chaseRange = 10f;       // Distance max de poursuite

    private float distance;
    private Animator animator;
    private bool isStunned = false; 
    private Rigidbody2D rb;
    private Vector2 direction;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        // Auto-find player si non assigné
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj;
        }
    }

    void Update()
    {
        // Arrêter le mouvement si touché ou stunné
        if (isStunned || player == null) 
        {
            if (rb != null) rb.velocity = Vector2.zero;
            return;
        }

        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < chaseRange)
        {
            direction = (player.transform.position - transform.position).normalized;

            // Stop si trop proche (évite les tremblements)
            if (distance <= stickDistance)
            {
                if (rb != null) rb.velocity = Vector2.zero;
            }

            // Met à jour l’animator
            if (animator != null)
            {
                animator.SetFloat("X", direction.x);
                animator.SetFloat("Y", direction.y);
            }
        }
        else
        {
            if (rb != null) rb.velocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (!isStunned && player != null && distance > stickDistance && distance < chaseRange)
        {
            if (rb != null)
                rb.velocity = direction * speed;
            else
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.fixedDeltaTime);
        }
    }

    // ⭐ Stun : l’ennemi s’arrête net et ne suit plus le joueur
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        if (rb != null) rb.velocity = Vector2.zero;
        Debug.Log($"{gameObject.name} stunné pour {duration}s");
        
        yield return new WaitForSeconds(duration);
        
        isStunned = false;
        Debug.Log($"{gameObject.name} n'est plus stunné");
    }

    // ⭐ Gizmos : visualisation des zones dans la Scene View
    private void OnDrawGizmosSelected()
    {
        // 🔵 Zone de détection
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        // 🔴 Zone de collage
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, stickDistance);
    }
}
