using UnityEngine;
using UnityEngine.InputSystem;

// Ce script gère l'attaque du joueur
public class PlayerAttack : MonoBehaviour
{
    private PlayerControls controls;
    private Animator animator;

    // Référence au script de mouvement pour obtenir la direction
    private PlayerMovement playerMovement;

    [Header("Attaque")]
    public float attackDuration = 0.5f;
    private float attackTimer;
    private bool isAttacking = false;

    // Game Object contenant le BoxCollider
    public GameObject attackHitbox;
    // Script qui gère l'activation
    private AttackHitboxController attackController;
    // Distance de décalage de la hitbox par rapport au joueur
    public float hitboxOffset = 0.5f;

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void OnEnable()
    {
        controls.Player.Enable();
        controls.Player.Attack.performed += OnAttackPerformed;
    }

    void OnDisable()
    {
        controls.Player.Attack.performed -= OnAttackPerformed;
        controls.Player.Disable();
    }

    void Start()
    {
        if (attackHitbox != null)
        {
            attackController = attackHitbox.GetComponent<AttackHitboxController>();
            attackHitbox.SetActive(true);
        }
    }

    void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                animator.SetBool("IsAttacking", false);
            }
        }
    }

    /// <summary>
    /// Fonction appelée par le système d'input pour l'attaque
    /// </summary>
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (!isAttacking && context.performed)
        {
            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetBool("IsAttacking", true);

            // Met à jour la direction de l'attaque en utilisant la dernière direction du mouvement
            Vector2 attackDirection = playerMovement.LastMovement;
            if (attackDirection == Vector2.zero)
            {
                // Si le joueur ne bouge pas, on utilise la dernière direction connue
                // (peut-être initialisée à une valeur par défaut comme Vector2.down)
                attackDirection = new Vector2(animator.GetFloat("X"), animator.GetFloat("Y"));
            }

            // Met à jour l'animator pour la direction de l'attaque
            animator.SetFloat("X", attackDirection.x);
            animator.SetFloat("Y", attackDirection.y);
            
            // Positionne la hitbox dans la bonne direction
            Vector3 offset = (Vector3)attackDirection.normalized * hitboxOffset;
            attackHitbox.transform.localPosition = offset;

            // Active la hitbox temporairement
            if (attackController != null)
            {
                attackController.ActivateHitbox();
            }
        }
    }
}