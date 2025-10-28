using UnityEngine;
using UnityEngine.InputSystem;

public class SimplePlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float runSpeed = 8f;
    
    [Header("Options")]
    public bool useCameraDirection = true;
    
    private CharacterController controller;
    private Vector3 velocity;
    private bool isRunning;
    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found! Please add CharacterController component to this GameObject.");
        }
    }
    
    void Update()
    {
        if (controller == null) return;
        
        // Get input using Input System
        float horizontal = 0f;
        float vertical = 0f;
        bool sprintPressed = false;
        
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            horizontal = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
            vertical = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
            sprintPressed = keyboard.leftShiftKey.isPressed;
        }
        else
        {
            Debug.LogWarning("No keyboard found!");
        }
        
        // Camera-relative basis
        Vector3 forward, right;
        if (useCameraDirection && Camera.main != null)
        {
            Transform cam = Camera.main.transform;
            forward = Vector3.ProjectOnPlane(cam.forward, Vector3.up).normalized;
            right   = Vector3.ProjectOnPlane(cam.right,   Vector3.up).normalized;
        }
        else
        {
            forward = Vector3.forward;
            right   = Vector3.right;
        }
        
        // Create movement vector (strafe + forward)
        Vector3 move = right * horizontal + forward * vertical;
        if (move.sqrMagnitude > 1f) move.Normalize();
        
        // Determine speed
        float currentSpeed = sprintPressed ? runSpeed : walkSpeed;
        
        // Apply movement
        controller.Move(move * currentSpeed * Time.deltaTime);
        
        
        velocity.y += Physics.gravity.y * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
        
        
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        
        
        isRunning = sprintPressed && move.magnitude > 0.1f;
        
        
        if (move.magnitude > 0.1f)
        {
            Debug.Log($"Simple Movement - Move: {move}, Speed: {currentSpeed}, Sprint: {sprintPressed}, Input: ({horizontal}, {vertical})");
        }
    }
    
    // AnimationController
    public bool IsMoving()
    {
        var keyboard = Keyboard.current;
        if (keyboard != null)
        {
            float horizontal = (keyboard.dKey.isPressed ? 1 : 0) - (keyboard.aKey.isPressed ? 1 : 0);
            float vertical = (keyboard.wKey.isPressed ? 1 : 0) - (keyboard.sKey.isPressed ? 1 : 0);
            return Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f;
        }
        return false;
    }
    
    public bool IsRunning()
    {
        return isRunning;
    }
}
