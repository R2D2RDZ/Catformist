using UnityEngine;
using UnityEngine.InputSystem;

public class Cat_Inputs : MonoBehaviour
{

    [Header("Input Settings")]
    [SerializeField] private InputActionReference eatAction;

    private PlayerInput playerInput;

    private Cat_Movement catMovementScr;
    private Cat_Attack catAttackScr;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        catMovementScr = GetComponent<Cat_Movement>();
        catAttackScr = GetComponent<Cat_Attack>();
    }

    void FixedUpdate()
    {
        if (catMovementScr != null && catMovementScr.stateStunned) return;

        Vector2 direction2D = playerInput.actions["Movement"].ReadValue<Vector2>();

        catMovementScr.direction2D = direction2D;
    }

    private void OnJumpClimb()
    {
        if (catMovementScr != null && catMovementScr.stateStunned) return;

        if(catMovementScr != null)
        { catMovementScr.Jump(); }
    }

    private void OnSlash()
    {
        Debug.Log("Slashing");
        if (catMovementScr != null && catMovementScr.stateStunned) return;

        if (catAttackScr != null)
        {
            Debug.Log("Perform Slash");
            catAttackScr.PerformSlash();
        }
    }

    public bool IsEatPressed()
    {
        if (catMovementScr != null && catMovementScr.stateStunned) return false;
        return playerInput.actions["Eat"].IsPressed();
    }

    public bool WasEatPressedThisFrame()
    {
        if (catMovementScr != null && catMovementScr.stateStunned) return false;
        return playerInput.actions["Eat"].WasPressedThisFrame();
    }

    public bool WasEatReleasedThisFrame()
    {
        if (catMovementScr != null && catMovementScr.stateStunned) return false;
        return playerInput.actions["Eat"].WasReleasedThisFrame();
    }
}
