using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementIsometric : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;
    private Vector2 lastMovement;

    private bool isAttacking = false;
    private float attackDuration = 0.5f;
    private float attackTimer;

    public GameObject AttackHitbox; // GameObject contenant le BoxCollider2D
    private AttackHitboxController attackController;

    public float hitboxOffset = 0.5f; // Distance de décalage de la hitbox par rapport au joueur

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        rb.gravityScale = 0;

        // Récupération du controller de la hitbox
        attackController = AttackHitbox.GetComponent<AttackHitboxController>();

        // Assure que le GameObject est actif (mais le collider est désactivé par le controller)
        AttackHitbox.SetActive(true);
    }

    void Update()
    {
        if (movement.sqrMagnitude > 0.01f)
        {
            lastMovement = movement.normalized;
        }

        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }

        bool isWalking = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);

        animator.SetFloat("X", isWalking ? movement.x : lastMovement.x);
        animator.SetFloat("Y", isWalking ? movement.y : lastMovement.y);
    }

    void FixedUpdate()
    {
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    public void OnMovement(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

public void OnAttack(InputValue value)
{
    // Vérifie que le joueur bouge avant de lancer une attaque
    if (!isAttacking && value.isPressed && movement.sqrMagnitude > 0.01f)
    {
        // Direction de l’attaque
        animator.SetFloat("X", lastMovement.x);
        animator.SetFloat("Y", lastMovement.y);

        isAttacking = true;
        attackTimer = attackDuration;
        animator.SetBool("IsAttacking", true);

        // Positionne la hitbox dans la bonne direction
        Vector3 offset = new Vector3(lastMovement.x, lastMovement.y, 0).normalized * hitboxOffset;
        AttackHitbox.transform.localPosition = offset;

        // Active la hitbox temporairement via le controller
        attackController.ActivateHitbox();
    }
}
}
