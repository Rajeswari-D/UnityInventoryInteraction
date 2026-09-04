using UnityEngine;

public class WorldItemDropper : MonoBehaviour
{
    public static WorldItemDropper Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform dropPoint;

    [Header("Drop Settings")]
    [SerializeField] private float forwardOffset = 0f;
    [SerializeField] private float dropHeight = 1.5f;

    [Header("Physics")]
    [SerializeField] private float itemMass = 1f;
    [SerializeField] private float drag = 0.5f;
    [SerializeField] private float angularDrag = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public bool TryDropItem(ItemData item, int quantity)
    {
        if (item == null || quantity <= 0)
        {
            return false;
        }

        if (item.WorldPrefab == null)
        {
            Debug.LogError(
                $"WorldItemDropper: No world prefab assigned for {item.DisplayName}."
            );

            return false;
        }

        if (dropPoint == null)
        {
            Debug.LogError(
                "WorldItemDropper: Drop Point is missing."
            );

            return false;
        }

        Vector3 spawnPosition =
            dropPoint.position +
            dropPoint.forward * forwardOffset +
            Vector3.up * dropHeight;

        WorldItem droppedItem =
            Instantiate(
                item.WorldPrefab,
                spawnPosition,
                Quaternion.identity
            );

        if (droppedItem == null)
        {
            return false;
        }

        droppedItem.SetItem(
            item,
            quantity
        );

        droppedItem.gameObject.SetActive(true);

        Rigidbody rigidbody =
            droppedItem.GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            rigidbody =
                droppedItem.gameObject.AddComponent<Rigidbody>();
        }

        rigidbody.mass = itemMass;
        rigidbody.linearDamping = drag;
        rigidbody.angularDamping = angularDrag;
        rigidbody.useGravity = true;
        rigidbody.isKinematic = false;

        Debug.Log(
            $"World item dropped: {item.DisplayName} x{quantity}"
        );

        return true;
    }
}