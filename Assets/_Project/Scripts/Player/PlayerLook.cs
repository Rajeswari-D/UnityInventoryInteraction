using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private InventoryUI inventoryUI;

    [Header("Look Settings")]
    [SerializeField] private float mouseSensitivity = 0.1f;
    [SerializeField] private float maxLookAngle = 85f;

    private float verticalRotation;

    private void Start()
    {
        LockCursor();
    }

    private void Update()
    {
        // Inventory open = allow mouse interaction.
        if (inventoryUI != null &&
            inventoryUI.IsOpen)
        {
            UnlockCursor();
            return;
        }

        // Normal gameplay = FPS mouse look.
        LockCursor();

        HandleMouseLook();
    }

    private void HandleMouseLook()
    {
        if (Mouse.current == null)
        {
            return;
        }

        Vector2 mouseDelta =
            Mouse.current.delta.ReadValue();

        float mouseX =
            mouseDelta.x * mouseSensitivity;

        float mouseY =
            mouseDelta.y * mouseSensitivity;

        // Horizontal rotation.
        transform.Rotate(
            Vector3.up * mouseX
        );

        // Vertical rotation.
        verticalRotation -= mouseY;

        verticalRotation =
            Mathf.Clamp(
                verticalRotation,
                -maxLookAngle,
                maxLookAngle
            );

        if (cameraHolder != null)
        {
            cameraHolder.localRotation =
                Quaternion.Euler(
                    verticalRotation,
                    0f,
                    0f
                );
        }
    }

    private void LockCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    private void UnlockCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    private void OnDisable()
    {
        UnlockCursor();
    }
}