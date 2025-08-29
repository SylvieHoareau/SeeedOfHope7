using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Ce script gère l'attaque du joueur
public class PlayerAttack : MonoBehaviour
{
    [Header("Dégâts du joueur")]
    public float baseDamage = 1f;          // dégâts normaux
    public float damageMultiplier = 1f;   // multiplicateur (cheat = x5 ou x10)

    private PlayerControls controls;
    private Animator animator;

    // Référence au script de mouvement pour obtenir la direction
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Attaque")]
    public float attackCooldown = 0.5f; // Temps entre chaque attaque
    public float attackDuration = 0.4f; // Durée de l'animation d'attaque
    
    // Le booléen est une variable privée, gérée uniquement par ce script.
    private bool isAttacking = false;
    private float lastAttackTime; // Pour gérer le cooldown

    [Header("Hitbox")]
    // GameObject qui contient le script AttackHitbox et le BoxCollider2D
    public GameObject hitboxObject;

    // Game Object contenant le BoxCollider
    private AttackHitbox attackHitbox;

    [Header("Debug")]
    public bool showDebugLogs = true;

    void Awake()
    {
        controls = new PlayerControls();
        animator = GetComponent<Animator>();

        if (playerMovement == null)
            Debug.LogError("PlayerMovement non assigné !");
            
        // Récupère la référence à l'AttackHitbox sur le HitboxObject
        if (hitboxObject != null)
        {
            attackHitbox = hitboxObject.GetComponent<AttackHitbox>();
            // Désactive la hitbox par défaut
            hitboxObject.SetActive(false);
        }
        else
        {
            Debug.LogError("L'objet de la hitbox n'est pas assigné dans l'inspecteur !");
        }
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

    void Update()
    {
       // Cheat code : toggle avec clavier (F12) ou manette (Select/Back)
        if (Keyboard.current.f12Key.wasPressedThisFrame || 
            Gamepad.current != null && Gamepad.current.selectButton.wasPressedThisFrame)
        {
            if (damageMultiplier == 1f)
            {
                damageMultiplier = 5f; // booste les dégâts
                Debug.Log("Cheat ACTIVÉ : dégâts x5");
            }
            else
            {
                damageMultiplier = 1f; // revient à la normale
                Debug.Log("Cheat DÉSACTIVÉ : dégâts normaux");
            }
        }
    }

    /// <summary>
    /// Fonction appelée par le système d'input pour l'attaque
    /// </summary>
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // Vérifie si le joueur est déjà en train d'attaquer ou si le cooldown n'est pas terminé
        if (isAttacking || Time.time < lastAttackTime + attackCooldown)
        {
            if (showDebugLogs)
                Debug.Log("Attaque impossible, cooldown actif.");
            return;
        }
        
        // Si le cooldown est terminé et le joueur n'est pas déjà en train d'attaquer
        StartCoroutine(AttackRoutine());
        lastAttackTime = Time.time;
    }

    private IEnumerator AttackRoutine()
    {
        // Démarre l'attaque
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        // Détermine la direction de l'attaque
        Vector2 attackDirection = playerMovement.LastMovement.normalized;
        if (attackDirection == Vector2.zero)
        {
            attackDirection = Vector2.down;
        }

        // Met à jour l'animator pour que l'animation d'attaque soit dans la bonne direction
        animator.SetFloat("X", attackDirection.x);
        animator.SetFloat("Y", attackDirection.y);

        // Active la hitbox
        if (attackHitbox != null)
        {
            attackHitbox.damage = baseDamage * damageMultiplier;
            hitboxObject.SetActive(true);
            attackHitbox.Activate(attackDirection);
        }

        if (showDebugLogs)
            Debug.Log("Hitbox activée dans la direction: " + attackDirection);

        // Attend la fin de l'animation d'attaque
        yield return new WaitForSeconds(attackDuration);

        // Désactive la hitbox et met fin à l'attaque
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
        }

        isAttacking = false;
        animator.SetBool("IsAttacking", false);
        
        if (showDebugLogs)
            Debug.Log($"Attaque terminée.");
    }
}