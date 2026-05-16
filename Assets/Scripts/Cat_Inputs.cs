using UnityEngine;
using UnityEngine.InputSystem;

public class Cat_Inputs : MonoBehaviour
{

    private PlayerInput playerInput;

    private Cat_Movement catMovementScr;


    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        catMovementScr = GetComponent<Cat_Movement>();
    }

    void FixedUpdate()
    {
        Vector2 direction2D = playerInput.actions["Movement"].ReadValue<Vector2>();

        catMovementScr.direction2D = direction2D;
    }

    private void OnJumpClimb()
    {
        if(catMovementScr != null)
        { catMovementScr.Jump(); }
    }
}
