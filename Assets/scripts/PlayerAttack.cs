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
    public float attackDuration = 0.4f;
    public float hitboxActiveStart = 0.1f; // Délai avant activation de la hitbox
    public float hitboxActiveDuration = 0.2f; // Durée d'activation de la hitbox
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

    [Header("Debug")]
    public bool showDebugLogs = true;

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
            hitboxObject.SetActive(true); // Désactivé par défaut
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
                EndAttack();
            }
        }
    }

    /// <summary>
    /// Fonction appelée par le système d'input pour l'attaque
    /// </summary>
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (isAttacking) return; // Empêche les attaques multiples

        StartAttack();
    }

    private void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackDuration;
        animator.SetBool("IsAttacking", true);

        // Direction d'attaque
        Vector2 attackDirection = playerMovement.CurrentMovement.sqrMagnitude > 0.01f
            ? playerMovement.CurrentMovement.normalized
            : playerMovement.LastMovement;

        if (attackDirection.sqrMagnitude < 0.01f)
        {
            attackDirection = Vector2.down;
        }

        // Met à jour l'animator
        animator.SetFloat("X", attackDirection.x);
        animator.SetFloat("Y", attackDirection.y);

        if (showDebugLogs)
        {
            Debug.Log($"Attaque lancée dans la direction: {attackDirection}");
        }

        // Active la hitbox avec un délai
        StartCoroutine(ActivateHitboxWithDelay(attackDirection));
    }

    private System.Collections.IEnumerator ActivateHitboxWithDelay(Vector2 direction)
    {
        // Attendre le délai avant d'activer la hitbox
        yield return new WaitForSeconds(hitboxActiveStart);

        if (attackHitbox != null && isAttacking)
        {
            hitboxObject.SetActive(true);
            attackHitbox.Activate(direction);

            if (showDebugLogs)
            {
                Debug.Log("Hitbox activée");
            }
        }

        // Garder la hitbox active pendant la durée spécifiée
        yield return new WaitForSeconds(hitboxActiveDuration);

        // Désactiver la hitbox
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
            if (showDebugLogs)
            {
                Debug.Log("Hitbox désactivée");
            }
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);

        // S'assurer que la hitbox est désactivée
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }
    }

    // Méthode appelée par Animation Event pour un timing précis
    public void OnAttackHit()
    {
        if (attackHitbox != null)
        {
            Vector2 attackDirection = playerMovement.LastMovement;
            hitboxObject.SetActive(true);
            attackHitbox.Activate(attackDirection);

            // Désactiver après un court délai
            Invoke(nameof(DeactivateHitbox), hitboxActiveDuration);
        }
    }
    
    private void DeactivateHitbox()
    {
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }
    }
}