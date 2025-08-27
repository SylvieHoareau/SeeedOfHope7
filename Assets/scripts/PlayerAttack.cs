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
    public float attackDuration = 0.2f;
    private float attackTimer;
    private bool isAttacking = false;

    [Header("Hitbox")]
    // GameObject qui contient le script AttackHitbox et le BoxCollider2D
    public GameObject hitboxObject; 

    // Game Object contenant le BoxCollider
    private AttackHitbox attackHitbox;
    // Script qui gère l'activation
    // private AttackHitboxController attackController;
    // Distance de décalage de la hitbox par rapport au joueur
    // public float hitboxOffset = 0.5f;

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();
         // On récupère la référence au script de mouvement sur le même GameObject
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
        if (hitboxObject != null)
        {
            attackHitbox = hitboxObject.GetComponent<AttackHitbox>();
            hitboxObject.SetActive(true);
        }
        else
        {
            Debug.LogError("L'objet de la hitbox n'est pas assigné dans l'inspecteur !");
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
        if (!isAttacking && playerMovement.CurrentMovement.sqrMagnitude > 0.01f)
        {
            // On peut attaquer même à l'arrêt.
            if (isAttacking) return;

            isAttacking = true;
            attackTimer = attackDuration;
            animator.SetBool("IsAttacking", true);

            // On utilise la dernière direction connue du script de mouvement.
            // LastMovement conserve la dernière direction même quand le joueur s'arrête.
            Vector2 attackDirection = playerMovement.LastMovement;

            // S'assure qu'il y a une direction par défaut si le jeu commence sans bouger.
            if (attackDirection.sqrMagnitude < 0.01f)
            {
                attackDirection = Vector2.down; // ou une autre direction par défaut
            }

            // Met à jour l'animator pour la direction de l'attaque
            animator.SetFloat("X", attackDirection.x);
            animator.SetFloat("Y", attackDirection.y);
            
            // On appelle la méthode Activate de notre hitbox
            // en lui passant la direction de l'attaque.
            if (attackHitbox != null)
            {
                attackHitbox.Activate(attackDirection);
            }

        }
    }
}