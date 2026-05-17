using UnityEngine;
using UnityEngine.InputSystem;

public class Cat_Inputs : MonoBehaviour
{
    public bool isEatPressed;
    public bool wasEatPressed;
    public bool wasEatReleased;

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
    void Update()
    {
        // Si el gato está aturdido, aseguramos que todo esté apagado
        if (catMovementScr != null && catMovementScr.stateStunned)
        {
            isEatPressed = false;
            wasEatPressed = false;
            wasEatReleased = false;
            return;
        }

        // Leemos directamente de la acción en el Update dinámico. 
        // Esto es 100% inmune a cómo esté configurado el botón en el editor de Unity.
        var eatAction = playerInput.actions["Eat"];
        if (eatAction != null)
        {
            isEatPressed = eatAction.IsPressed();

            if (eatAction.WasPressedThisFrame()) wasEatPressed = true;
            if (eatAction.WasReleasedThisFrame()) wasEatReleased = true;
        }
    }

    // Se ejecuta al final del frame para apagar los gatillos automáticamente
    void LateUpdate()
    {
        wasEatPressed = false;
        wasEatReleased = false;
    }
}
