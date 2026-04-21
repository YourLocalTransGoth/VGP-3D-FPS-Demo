using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Teleport : MonoBehaviour
{
    [SerializeField] private Teleport destination;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private float exitOffset = 1f;
    [SerializeField] private float cooldownSeconds = 0.2f;

    private Collider portalCollider;

    private void Awake()
    {
        portalCollider = GetComponent<Collider>();
        portalCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (destination == null)
        {
            return;
        }

        Transform traveler = other.transform.root;

        if (!IsValidTraveler(other, traveler))
        {
            return;
        }

        TeleportStamp stamp = traveler.GetComponent<TeleportStamp>();
        if (stamp == null)
        {
            stamp = traveler.gameObject.AddComponent<TeleportStamp>();
        }

        if (!stamp.CanTeleport(cooldownSeconds))
        {
            return;
        }

        traveler.position = destination.transform.position + destination.transform.forward * exitOffset;
        stamp.MarkTeleported();
    }

    private bool IsValidTraveler(Collider other, Transform traveler)
    {
        if (string.IsNullOrEmpty(targetTag))
        {
            return true;
        }

        if (other.CompareTag(targetTag) || traveler.CompareTag(targetTag))
        {
            return true;
        }

        if (targetTag == "Player" && traveler.GetComponentInChildren<PlayerMovement>() != null)
        {
            return true;
        }

        return false;
    }
}

public class TeleportStamp : MonoBehaviour
{
    private float lastTeleportTime = -1000f;

    public bool CanTeleport(float cooldown)
    {
        return Time.time - lastTeleportTime >= cooldown;
    }

    public void MarkTeleported()
    {
        lastTeleportTime = Time.time;
    }
}
