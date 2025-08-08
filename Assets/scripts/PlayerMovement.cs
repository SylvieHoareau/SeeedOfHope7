using UnityEngine;
using UnityEngine.InputSystem;

// Ce script gère le déplacement et l'attaque du joueur
public class PlayerMovementIsometric : MonoBehaviour
{
    // Vitesse de déplacement
    public float moveSpeed = 5f;
    // Gestion des inputs
    private PlayerControls controls;
    // Physique du joueur
    private Rigidbody2D rb;
    // Animation du joueur
    private Animator animator;
    // Direction du mouvement
    private Vector2 movement;
    // Denière direction utilisée
    private Vector2 lastMovement;

    // Indique si le joueur marche
    private bool isWalking = false;
    // Indique si le joueur attaque
    private bool isAttacking = false;
    // Durée de l'animation d'attaque
    private float attackDuration = 0.5f;
    // Timer pour l'attaque
    private float attackTimer;

    // Game Object contenant le BoxCollider
    public GameObject Attackhitbox; 
    // Script qui gère l'activation 
    private AttackHitboxController attackController;

    // Distance de décalage de la hitbox par rapport au joueur
    public float hitboxOffset = 0.5f;

    // Début des modifications 
    void Awake()
    {
        // Initialise les contrôles
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => movement = Vector2.zero;
    }

    void OnDisable()
    {
        controls.Player.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Pas besoin de gravité pour un jeu 2D isométrique
        rb.gravityScale = 0;

        // Récupération du controller de la hitbox
        attackController = Attackhitbox.GetComponent<AttackHitboxController>();

        // Assure que le GameObject est actif (mais le collider est désactiver par le controller)
        Attackhitbox.SetActive(true);
    }

    void Update()
    {
        // Mémorise la dernière direction utilisée
        if (movement.sqrMagnitude > 0.01f)
        {
            lastMovement = movement.normalized;
        }

        // Gestion du timer d'attaque
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }

        // Mets à jour les paramètres du blend tree pour l'animation
        animator.SetFloat("X", isWalking ? movement.x : lastMovement.x);
        animator.SetFloat("Y", isWalking ? movement.y : lastMovement.y);

        // Détecte si le joueur marche
        isWalking = movement.sqrMagnitude > 0.01f; // sqrMagnitude évite les erreurs de flottants
        animator.SetBool("IsWalking", isWalking);

    }

    void FixedUpdate()
    {
        // Applique le mouvement au Rigidbody 2D
        rb.linearVelocity = movement.normalized * moveSpeed;

    }

    // Méthode appelée par le système d'input pour le déplacement
    public void OnMovement(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    // Méthode appelée par le système d'input pour l'attaque
    public void OnAttack(InputValue value)
    {
        if (!isAttacking && value.isPressed)
        {
            // Met à jour la direction de l'attaque dans l'animator
            animator.SetFloat("X", lastMovement.x);
            animator.SetFloat("Y", lastMovement.y);

            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetBool("IsAttacking", true);

            // Positionne la hitbox dans la bonne direction
            Vector3 offset = new Vector3(lastMovement.x, lastMovement.y, 0).normalized * hitboxOffset;
            Attackhitbox.transform.localPosition = offset;

            // Active la hitbox temporairement via le controller
            attackController.ActivateHitbox();
        }
    }

}
