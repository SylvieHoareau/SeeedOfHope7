using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 2f;

    private float distance;
    private Animator animator;
    private Vector2 movement;
    private bool isTouchingPlayer = false;
    private bool isStunned = false; // ⭐ AJOUTÉ : État de stun
    private Rigidbody2D rb;

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

    private void OnMovement(InputValue value)
    {
        movement = value.Get<Vector2>();

        if (movement.x != 0 || movement.y != 0)
        {
            animator.SetFloat("X", movement.x);
            animator.SetFloat("Y", movement.y);
        }
    }

    void Update()
    {
        // ⭐ MODIFIÉ : Arrêter le mouvement si touché ou stunné
        if (isTouchingPlayer || isStunned || player == null) 
        {
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
            return;
        }

        distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < 3f)
        {
            Vector2 direction = (player.transform.position - transform.position).normalized;
            
            // Utilise le Rigidbody pour un mouvement plus fluide
            if (rb != null)
            {
                rb.linearVelocity = direction * speed;
            }
            else
            {
                // Fallback si pas de Rigidbody2D
                transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
            }

            // Met à jour l'animator avec la direction
            animator.SetFloat("X", direction.x);
            animator.SetFloat("Y", direction.y);
        }
        else
        {
            // Arrête le mouvement si trop loin
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = true;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTouchingPlayer = false;

            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Dynamic;
            }
        }
    }

    // ⭐ AJOUTÉ : Fonction publique pour le stun
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        
        // Arrête le mouvement
        if (rb != null)
            rb.linearVelocity = Vector2.zero;
            
        Debug.Log($"{gameObject.name} stunné pour {duration}s");
        
        yield return new WaitForSeconds(duration);
        
        isStunned = false;
        Debug.Log($"{gameObject.name} n'est plus stunné");
    }
}