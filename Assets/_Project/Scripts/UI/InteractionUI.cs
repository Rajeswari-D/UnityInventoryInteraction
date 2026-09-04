using TMPro;
using UnityEngine;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private PlayerInteractor interactor;
    [SerializeField] private GameObject interactionPanel;
    [SerializeField] private TMP_Text interactionText;

    private void OnEnable()
    {
        if (interactor != null)
        {
            interactor.InteractableChanged += HandleInteractableChanged;
        }
    }

    private void OnDisable()
    {
        if (interactor != null)
        {
            interactor.InteractableChanged -= HandleInteractableChanged;
        }
    }

    private void Start()
    {
        Hide();
    }

    private void HandleInteractableChanged(IInteractable interactable)
    {
        if (interactable == null)
        {
            Hide();
            return;
        }

        interactionText.text = interactable.GetInteractionPrompt();
        interactionPanel.SetActive(true);
    }

    private void Hide()
    {
        if (interactionPanel != null)
        {
            interactionPanel.SetActive(false);
        }
    }
}