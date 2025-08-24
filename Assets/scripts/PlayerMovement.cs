using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class PlayerMovement : MonoBehaviour
{
    private PlayerControls controls;
    private Rigidbody2D rb;
    private Animator animator;

    [Header("Paramètres de mouvement")]
    [SerializeField] private float moveSpeed = 5f;


    // 'movement' stocke la direction lue à chaque frame
    private Vector2 movement;
    // Pour les animations quand le joueur est à l'arrêt
    public Vector2 LastMovement { get; private set; }
    private bool isWalking = false;

    private void Awake()
    {
        // Initialisation des contrôles
        controls = new PlayerControls();

        // Récupération des composants
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Mettre la gravité à 0 pour un jeu en vue de dessus
        rb.gravityScale = 0;

        if (rb == null)
            Debug.LogError("Rigidbody2D non trouvé sur le joueur !");
    }

    private void OnEnable()
    {
        // On active le groupe d'action Player
        controls.Player.Enable();

        // Abonnement aux événements de mouvement
        // controls.Player.Move.performed += OnMove;
        // controls.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        // controls.Player.Move.performed -= OnMove;
        // controls.Player.Move.canceled -= OnMove;

        // Désactivation des contrôles
        controls.Player.Disable();
    }

    // private void OnMove(InputAction.CallbackContext ctx)
    // {
    //     movement = ctx.ReadValue<Vector2>();

    //     // Debug : afficher les valeurs de mouvement dans la console
    //     Debug.Log($"Move input: {movement} | Magnitude: {movement.magnitude}");

    //     // Mise à jour de la dernière direction si le joueur bouge
    //     if (movement.sqrMagnitude > 0.01f)
    //     {
    //         LastMovement = movement.normalized;
    //     }

    //     // Mise à jour de l'animator
    //     isWalking = movement.sqrMagnitude > 0.01f;
    //     animator.SetBool("IsWalking", isWalking);
    //     animator.SetFloat("X", isWalking ? movement.x : LastMovement.x);
    //     animator.SetFloat("Y", isWalking ? movement.y : LastMovement.y);
    // }

    // La méthode Update est appelée à chaque frame.
    // Idéale pour lire les inputs qui doivent être réactifs.
    private void Update()
    {
        // On lit la valeur de l'action 'Move' à chaque frame.
        // Si une touche est pressée, ça retournera (par ex.) (1, 0).
        // Si rien n'est pressé, ça retournera (0, 0).
        movement = controls.Player.Move.ReadValue<Vector2>();

        // On met à jour les animations en fonction du mouvement.
        UpdateAnimator();
    }

    private void FixedUpdate()
    {
        // On applique le mouvement au Rigidbody2D.
        // On normalise le vecteur pour que le mouvement en diagonale ne soit pas plus rapide.
        rb.linearVelocity = movement.normalized * moveSpeed;
    }

    // Une petite fonction pour garder le code de l'animation propre.
    private void UpdateAnimator()
    {
        // On vérifie si le joueur est en train de bouger.
        // 'sqrMagnitude' est un peu plus performant que 'magnitude'. Bon réflexe !
        bool isWalking = movement.sqrMagnitude > 0.01f;

        // On passe l'information à l'Animator.
        animator.SetBool("IsWalking", isWalking);

        // Si le joueur bouge, on met à jour la direction de l'animation.
        if (isWalking)
        {
            LastMovement = movement.normalized; // On sauvegarde la dernière direction
            animator.SetFloat("X", movement.x);
            animator.SetFloat("Y", movement.y);
        }
        // Si le joueur est à l'arrêt, l'animation d'idle utilisera la dernière direction connue.
        else
        {
            // On s'assure que les paramètres X et Y ne changent pas quand on s'arrête
            // pour que l'animation "idle" regarde dans la bonne direction.
            animator.SetFloat("X", LastMovement.x);
            animator.SetFloat("Y", LastMovement.y);
        }
    }
}
