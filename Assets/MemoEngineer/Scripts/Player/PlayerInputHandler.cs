using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public float MoveInput { get; private set; }

    // ------------------ Called by Unity Input System ------------------
    public void OnMove(InputAction.CallbackContext context)
    {
        MoveInput = context.ReadValue<float>();
    }
}