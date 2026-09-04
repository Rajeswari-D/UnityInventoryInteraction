using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float interactionRange = 3f;
    [SerializeField] private LayerMask interactionLayers = ~0;

    private IInteractable currentInteractable;

    public IInteractable CurrentInteractable => currentInteractable;

    public event Action<IInteractable> InteractableChanged;

    private void Update()
    {
        DetectInteractable();
        HandleInteractionInput();
    }

    private void DetectInteractable()
    {
        IInteractable detectedInteractable = null;

        if (playerCamera != null)
        {
            Ray ray = new Ray(
                playerCamera.transform.position,
                playerCamera.transform.forward
            );

            if (Physics.Raycast(
                    ray,
                    out RaycastHit hit,
                    interactionRange,
                    interactionLayers))
            {
                detectedInteractable =
                    hit.collider.GetComponentInParent<IInteractable>();

                if (detectedInteractable != null &&
                    !detectedInteractable.CanInteract())
                {
                    detectedInteractable = null;
                }
            }
        }

        if (detectedInteractable == currentInteractable)
        {
            return;
        }

        currentInteractable = detectedInteractable;

        InteractableChanged?.Invoke(currentInteractable);
    }

    private void HandleInteractionInput()
    {
        if (currentInteractable == null)
        {
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            currentInteractable.Interact();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
        {
            return;
        }

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward * interactionRange
        );
    }
}