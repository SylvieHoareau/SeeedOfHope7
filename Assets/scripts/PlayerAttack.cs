using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

// Ce script gère l'attaque du joueur
public class PlayerAttack : MonoBehaviour
{
    private PlayerControls controls;
    private Animator animator;

    // Référence au script de mouvement pour obtenir la direction
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Attaque")]
    public float attackDuration = 0.4f;
    public float hitboxActiveDuration = 0.2f; // Durée d'activation de la hitbox
    // private float attackTimer;
    private bool isAttacking = false;

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
        // animator = GetComponent<Animator>();
        // // On récupère la référence au script de mouvement sur le même GameObject
        // playerMovement = GetComponent<PlayerMovement>();
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
        // On s'assure que toutes les références requises sont bien là
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Composant Animator manquant sur " + gameObject.name + " !");
            return;
        }

        // Si playerMovement n'est pas assigné via l'Inspector, on tente de le récupérer
        // Note : Cette méthode est moins fiable si les scripts sont sur des objets différents
        if (playerMovement == null)
        {
            playerMovement = GetComponent<PlayerMovement>();
            if (playerMovement == null)
            {
                Debug.LogError("Le script PlayerMovement est manquant sur " + gameObject.name + ". Veuillez l'ajouter ou l'assigner dans l'Inspector.");
                return; // On arrête le Start() pour éviter les erreurs
            }
        }

        // Initialisation de la hitbox
        if (hitboxObject != null)
        {
            attackHitbox = hitboxObject.GetComponent<AttackHitbox>();
            // La hitBox doit être désactivé au démarrage
            hitboxObject.SetActive(false);

            if (showDebugLogs)
                Debug.Log("AttackHitbox trouvée et désactivée");
        }
        else
        {
            Debug.LogError("L'objet de la hitbox n'est pas assigné dans l'inspecteur !");
        }

    }

    void Update()
    {
       // Il n'est plus nécessaire de gérer le timer ici si la durée de l'animation le fait déjà.
        // Si vous voulez une "cooldown" indépendant de l'animation, remettez le timer.
        // Pour l'instant, on se fie à l'état isAttacking.
    }

    /// <summary>
    /// Fonction appelée par le système d'input pour l'attaque
    /// </summary>
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        // On vérifie les dépendances avant de lancer la coroutine
        if (isAttacking || playerMovement == null || animator == null) 
        {
            if (playerMovement == null) Debug.LogError("playerMovement est null ! Impossible d'attaquer.");
            if (animator == null) Debug.LogError("animator est null ! Impossible d'attaquer.");
            return;
        }

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        animator.SetBool("IsAttacking", true);

        // On récupère la dernière direction connue du script de mouvement
        Vector2 attackDirection = playerMovement.LastMovement;

        // Met à jour l'animator pour que l'animation d'attaque soit dans la bonne direction
        animator.SetFloat("X", attackDirection.x);
        animator.SetFloat("Y", attackDirection.y);

        if (showDebugLogs)
            Debug.Log($"Attaque lancée dans la direction: {attackDirection}");
        
        // Active la hitbox
        ActivateHitbox(attackDirection);

        // Attend la fin de la durée d'attaque
        yield return new WaitForSeconds(attackDuration);

        // Termine l'attaque
        isAttacking = false;
        animator.SetBool("IsAttacking", false);
    }

     // ✅ SIMPLIFIÉ: Méthode unique pour activer la hitbox
    private void ActivateHitbox(Vector2 direction)
    {
        if (attackHitbox != null)
        {
            hitboxObject.SetActive(true);
            attackHitbox.Activate(direction);

            if (showDebugLogs)
                Debug.Log("Hitbox activée immédiatement");

            // Désactiver après la durée
            StartCoroutine(HitboxDeactivationRoutine()); // ⭐ AMÉLIORATION : On utilise la coroutine
        }
    }
    
    // Coroutine pour gérer l'activation/désactivation de la hitbox
    private IEnumerator HitboxActivationRoutine()
    {
        // La hitbox est déjà activée avant d'appeler la coroutine
        yield return new WaitForSeconds(hitboxActiveDuration);
        StartCoroutine(HitboxDeactivationRoutine());
    }

     private IEnumerator HitboxDeactivationRoutine()
    {
        yield return new WaitForSeconds(hitboxActiveDuration);
        if (hitboxObject != null)
        {
            hitboxObject.SetActive(false);
            if (showDebugLogs)
                Debug.Log("Hitbox désactivée par la coroutine");
        }
    }

    private void EndAttack()
    {
        isAttacking = false;
        animator.SetBool("IsAttacking", false);

        // S'assurer que la hitbox est désactivée
        StartCoroutine(HitboxDeactivationRoutine());
    }

   // ✅ OPTIONNEL: Pour Animation Events si vous en avez besoin
    public void OnAttackHit()
    {
        // On s'assure que playerMovement n'est pas null avant de l'utiliser
        if (playerMovement == null) {
            Debug.LogError("playerMovement est null ! Impossible de gérer le hit d'attaque.");
            return;
        }

        Vector2 attackDirection = playerMovement.LastMovement;
        if (attackDirection.sqrMagnitude < 0.01f)
            attackDirection = Vector2.down;
            
        ActivateHitbox(attackDirection);
    }
    
}