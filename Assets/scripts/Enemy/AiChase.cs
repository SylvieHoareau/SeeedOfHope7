using UnityEngine;
using System.Collections;

public class AiChase : MonoBehaviour
{
    public GameObject player;
    public float speed = 7f;
    public float animationSmooth = 0.1f;

    private Animator animator;
    private Rigidbody2D rb;

    private bool isTouchingPlayer = false;
    private bool isStunned = false;

    private float currentX = 0f;
    private float currentY = 0f;
    private float velocityX = 0f;
    private float velocityY = 0f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                player = playerObj;
        }
    }

    void Update()
    {
        if (isTouchingPlayer || isStunned || player == null) 
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 direction = ((Vector2)player.transform.position - rb.position).normalized;
        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < 3f)
        {
            // Axe dominant pour Blend Tree (animation)
            Vector2 snapDir = Mathf.Abs(direction.x) > Mathf.Abs(direction.y)
                ? new Vector2(Mathf.Sign(direction.x), 0)
                : new Vector2(0, Mathf.Sign(direction.y));

            currentX = Mathf.SmoothDamp(currentX, snapDir.x, ref velocityX, animationSmooth);
            currentY = Mathf.SmoothDamp(currentY, snapDir.y, ref velocityY, animationSmooth);

            animator.SetFloat("X", currentX);
            animator.SetFloat("Y", currentY);

            // Déplacement via velocity
            rb.linearVelocity = direction * speed;
        }
        else
        {
            // Trop loin : stop
            rb.linearVelocity = Vector2.zero;

            // Idle : ramène X/Y à 0
            currentX = Mathf.SmoothDamp(currentX, 0, ref velocityX, animationSmooth);
            currentY = Mathf.SmoothDamp(currentY, 0, ref velocityY, animationSmooth);

            animator.SetFloat("X", currentX);
            animator.SetFloat("Y", currentY);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            isTouchingPlayer = false;
    }

    // Fonction publique pour hit stun
    public void Stun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        rb.linearVelocity = Vector2.zero;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
}
