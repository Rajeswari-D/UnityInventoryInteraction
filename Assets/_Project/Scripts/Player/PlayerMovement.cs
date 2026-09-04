using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float gravity = -20f;

    private CharacterController characterController;

    private Vector3 velocity;

    private void Awake()
    {
        characterController =
            GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleMovement();
        ApplyGravity();
    }

    private void HandleMovement()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        Vector2 input = Vector2.zero;

        if (Keyboard.current.wKey.isPressed)
        {
            input.y += 1f;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            input.y -= 1f;
        }

        if (Keyboard.current.aKey.isPressed)
        {
            input.x -= 1f;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            input.x += 1f;
        }

        input =
            Vector2.ClampMagnitude(input, 1f);

        Vector3 movement =
            transform.right * input.x +
            transform.forward * input.y;

        characterController.Move(
            movement * moveSpeed * Time.deltaTime
        );
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded &&
            velocity.y < 0f)
        {
            velocity.y = -2f;
        }

        velocity.y +=
            gravity * Time.deltaTime;

        characterController.Move(
            velocity * Time.deltaTime
        );
    }
}