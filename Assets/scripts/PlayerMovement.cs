using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementIsometric : MonoBehaviour
{
    public float moveSpeed = 5f;

    // Début de modifications
    private PlayerControls controls;
    // Fin de modifications
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 movement;

    // Début des modifications 
    void Awake()
    {
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
    }

    void Update()
    {
        // Envoie les directions au blend tree
        animator.SetFloat("X", movement.x);
        animator.SetFloat("Y", movement.y);

        // Détection continue du déplacement
        bool isWalking = movement.sqrMagnitude > 0.01f; // sqrMagnitude évite les erreurs de flottants
        animator.SetBool("IsWalking", isWalking);

    }

    void FixedUpdate()
    {
        // Applique le mouvement
        rb.linearVelocity = movement.normalized * moveSpeed;

    }

    // public void OnMovement(InputValue value)
    // {
    //     movement = value.Get<Vector2>();
    // }
}
