using UnityEngine;
using UnityEngine.InputSystem;

// Ce script gère le déplacement et l'attaque du joueur
public class PlayerMovement : MonoBehaviour
{

    // Réfère l'objet "PlayerControls" qui gère les entrées du joueur
    private PlayerControls controls;
    // Physique du joueur
    private Rigidbody2D rb;

    // Vitesse de déplacement du joueur
    [Header("Paramètres de mouvement")]
    [SerializeField] private float moveSpeed = 5f;

    // Direction du mouvement
    private Vector2 movement;
    // Denière direction utilisée
    private Vector2 lastMovement;

    // Vecteur qui stocke la direction du mouvement (lecture de l'entrée du clavier/joystick)
    // private Vector2 moveInput;


    // Animation du joueur
    [Header("Composants")]
    [SerializeField] private Animator animator;

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

        // On récupère les composants nécessaires (Rigidbody2D et Animator)
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Si le Rigidbody2D est manquant, on le signale
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D non trouvé sur le joueur. Veuillez en ajouter un.");
        }
    }

    void OnEnable()
    {
        // On active l'action "Move" pour que le joueur puisse bouger.
        controls.Player.Enable();

        // On s'abonne aux événements de mouvement pour mettre à jour la variable "movement"
        controls.Player.Move.performed += OnMovement;
        controls.Player.Move.canceled += OnMovement;
        // controls.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        // controls.Player.Move.canceled += ctx => movement = Vector2.zero;
    }

    void OnDisable()
    {
        // On se désabonne pour éviter les fuites de mémoire (très important !)
        controls.Player.Move.performed -= OnMovement;
        controls.Player.Move.canceled -= OnMovement;

        // On désactive l'action "Move" quand le script est désactivé
        // (par exemple, si le joueur est dans un menu)
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

    // Méthode appelée par le système d'input pour le déplacement
    // La méthode qui sera appelée par le système d'entrée
    private void OnMovement(InputAction.CallbackContext value)
    {
        movement = value.ReadValue<Vector2>();

        // La dernière direction est mise à jour uniquement si le joueur bouge
        if (movement.sqrMagnitude > 0.01f)
        {
            lastMovement = movement.normalized;
        }

        // On met à jour l'état "IsWalking" de l'animator ici, directement
        isWalking = movement.sqrMagnitude > 0.01f;
        animator.SetBool("IsWalking", isWalking);
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

    void FixedUpdate()
    {
        // Applique le mouvement au Rigidbody 2D
        rb.linearVelocity = movement.normalized * moveSpeed;
    }
    
    void Update()
    {
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
    }

}
