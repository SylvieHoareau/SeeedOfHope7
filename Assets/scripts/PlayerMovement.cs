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


     // Propriété publique pour le mouvement actuel
    public Vector2 CurrentMovement { get; private set; }
    // Pour les animations quand le joueur est à l'arrêt
    public Vector2 LastMovement { get; private set; }
    // private bool isWalking = false;

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

    // La méthode Update est appelée à chaque frame.
    // Idéale pour lire les inputs qui doivent être réactifs.
    private void Update()
    {
        // On lit la valeur de l'action 'Move' à chaque frame.
        // Si une touche est pressée, ça retournera (par ex.) (1, 0).
        // Si rien n'est pressé, ça retournera (0, 0).
        CurrentMovement = controls.Player.Move.ReadValue<Vector2>();

        // On met à jour la dernière direction si le joueur bouge
        if (CurrentMovement.sqrMagnitude > 0.01f)
        {
            LastMovement = CurrentMovement.normalized;
        }

        // On met à jour les animations en fonction du mouvement.
        UpdateAnimator();
    }

    // Réservé à la physique
    private void FixedUpdate()
    {
        // On applique le mouvement au Rigidbody2D.
        // On normalise le vecteur pour que le mouvement en diagonale ne soit pas plus rapide.
        rb.linearVelocity = CurrentMovement.normalized * moveSpeed;
    }

     // Cette méthode est appelée par le Player Input component
    // public void OnMovement(InputValue value)
    // {
    //     CurrentMovement = value.Get<Vector2>();
    // }

    // Une petite fonction pour garder le code de l'animation propre.
    private void UpdateAnimator()
    {
        // On vérifie si le joueur est en train de bouger.
        // 'sqrMagnitude' est un peu plus performant que 'magnitude'. Bon réflexe !
        bool isWalking = CurrentMovement.sqrMagnitude > 0.01f;

        // On passe l'information à l'Animator.
        animator.SetBool("IsWalking", isWalking);

        // Si le joueur bouge, on met à jour la direction de l'animation.
        
        // On utilise LastMovement pour définir la direction, que le joueur soit en mouvement ou à l'arrêt.
        // C'est plus simple et évite les changements brusques à l'arrêt.
        animator.SetFloat("X", LastMovement.x);
        animator.SetFloat("Y", LastMovement.y);
    }
}
