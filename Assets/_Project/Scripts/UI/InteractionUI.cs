using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private InventoryUI inventoryUI;

    [Header("UI")]
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TMP_Text interactionText;

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.InteractableChanged +=
                HandleInteractableChanged;
        }
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.InteractableChanged -=
                HandleInteractableChanged;
        }
    }

    private void Start()
    {
        ShowOpenInventoryHint();
    }

    private void Update()
    {
        if (inventoryUI == null)
        {
            return;
        }

        // Don't override the pickup prompt while
        // the player is looking at an interactable.
        if (interactor != null &&
            interactor.CurrentInteractable != null)
        {
            return;
        }

        if (inventoryUI.IsOpen)
        {
            ShowCloseInventoryHint();
        }
        else
        {
            ShowOpenInventoryHint();
        }
    }

    private void HandleInteractableChanged(
        IInteractable interactable)
    {
        if (interactable == null)
        {
            UpdateInventoryHint();
            return;
        }

        if (interactionText != null)
        {
            interactionText.text =
                interactable.GetInteractionPrompt();
        }

        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }
    }

    private void UpdateInventoryHint()
    {
        if (inventoryUI == null)
        {
            ShowOpenInventoryHint();
            return;
        }

        if (inventoryUI.IsOpen)
        {
            ShowCloseInventoryHint();
        }
        else
        {
            ShowOpenInventoryHint();
        }
    }

    private void ShowOpenInventoryHint()
    {
        if (interactionText != null)
        {
            interactionText.text =
                "TAB - Open Inventory";
        }

        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }
    }

    private void ShowCloseInventoryHint()
    {
        if (interactionText != null)
        {
            interactionText.text =
                "TAB - Close Inventory";
        }

        if (interactionPanel != null)
        {
            interactionPanel.SetActive(true);
        }
    }
}