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

    public IInteractable CurrentInteractable =>
        currentInteractable;

    public event Action<IInteractable>
        InteractableChanged;

    public event Action<IInteractable>
        InteractionSucceeded;

    private void Update()
    {
        DetectInteractable();
        HandleInteractionInput();
    }

    private void DetectInteractable()
    {
        IInteractable detectedInteractable = null;

        if (playerCamera == null)
        {
            SetCurrentInteractable(null);
            return;
        }

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
                hit.collider
                    .GetComponentInParent<IInteractable>();

            if (detectedInteractable != null &&
                !detectedInteractable.CanInteract())
            {
                detectedInteractable = null;
            }
        }

        SetCurrentInteractable(
            detectedInteractable
        );
    }

    private void SetCurrentInteractable(
        IInteractable interactable)
    {
        if (currentInteractable ==
            interactable)
        {
            return;
        }

        currentInteractable =
            interactable;

        InteractableChanged?.Invoke(
            currentInteractable
        );
    }

    private void HandleInteractionInput()
    {
        if (currentInteractable == null)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        }

        if (!Keyboard.current.eKey
                .wasPressedThisFrame)
        {
            return;
        }

        IInteractable interactable =
            currentInteractable;

        bool succeeded =
            interactable.Interact();

        if (!succeeded)
        {
            return;
        }

        // Tell UI / other systems that
        // the interaction succeeded.
        InteractionSucceeded?.Invoke(
            interactable
        );

        // Clear the old reference.
        // The next Update will perform a
        // completely fresh raycast.
        SetCurrentInteractable(null);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerCamera == null)
        {
            return;
        }

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawRay(
            playerCamera.transform.position,
            playerCamera.transform.forward *
            interactionRange
        );
    }
}