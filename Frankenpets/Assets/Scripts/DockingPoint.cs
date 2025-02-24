using UnityEngine;

public class DockingPoint : MonoBehaviour
{
    [Header("Docking Configuration")]
    public string acceptedHalfType; // "catFront", "dogFront", "catBack", "dogBack"
    public bool isFrontDockingPoint; // true for front halves, false for back halves
    public Transform alignmentTransform; // Where the pet half should align to when docked
    
    [Header("References")]
    public PetStationManager stationManager;
    private GameObject dockedHalf;
    private bool isDocked = false;

    void Start()
    {
        // Ensure we have a trigger collider
        if (GetComponent<Collider>() == null)
        {
            Debug.LogError($"DockingPoint {gameObject.name} missing Collider component!");
            return;
        }
        
        if (!GetComponent<Collider>().isTrigger)
        {
            Debug.LogError($"DockingPoint {gameObject.name} collider must be set to trigger!");
            return;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger entered on {gameObject.name} by {other.gameObject.name}");
        
        // Check if the colliding object is a pet half's magnet
        if (!other.gameObject.CompareTag("PetMagnet"))
        {
            Debug.Log($"Object {other.gameObject.name} does not have PetMagnet tag");
            return;
        }

        // Get the pet half GameObject (parent of the magnet)
        GameObject petHalf = other.transform.parent.gameObject;
        Debug.Log($"Pet half detected: {petHalf.name}");
        
        // Verify this is a valid docking attempt
        if (!CanDock(petHalf))
        {
            Debug.Log($"Cannot dock {petHalf.name} at {gameObject.name}");
            return;
        }

        Debug.Log($"Attempting to dock {petHalf.name} at {gameObject.name}");
        // Attempt to dock the pet half
        TryDock(petHalf);
    }

    private bool CanDock(GameObject petHalf)
    {
        // Check if something is already docked
        if (isDocked)
            return false;

        // Verify this is the correct type of pet half
        if (petHalf.name != acceptedHalfType)
            return false;

        // Check if the half is already docked somewhere else
        if (petHalf.GetComponent<Rigidbody>().isKinematic)
            return false;

        // Additional validation through station manager
        return stationManager.ValidateDockingAttempt(this, petHalf);
    }

    private void TryDock(GameObject petHalf)
    {
        // Notify station manager of docking attempt
        if (!stationManager.RequestDocking(this, petHalf))
            return;

        // Lock the pet half in place
        LockPetHalf(petHalf);

        // Update state
        dockedHalf = petHalf;
        isDocked = true;

        // Notify station manager that docking is complete
        stationManager.OnDockingComplete(this, petHalf);
    }

    private void LockPetHalf(GameObject petHalf)
    {
        if (alignmentTransform == null)
        {
            Debug.LogError($"Alignment Transform not set for docking point {gameObject.name}!");
            return;
        }

        Rigidbody rb = petHalf.GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError($"No Rigidbody component found on {petHalf.name}!");
            return;
        }
        
        // Make kinematic to lock in place
        rb.isKinematic = true;
        
        // Align to docking point
        petHalf.transform.position = alignmentTransform.position;
        petHalf.transform.rotation = alignmentTransform.rotation;
    }

    public void UndockCurrentHalf()
    {
        if (!isDocked || dockedHalf == null)
            return;

        // Get the Rigidbody
        Rigidbody rb = dockedHalf.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Re-enable physics
            rb.isKinematic = false;
            rb.useGravity = true;
            
            // Make sure constraints are properly set
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            
            // Give it a small impulse to prevent sticking
            rb.AddForce(transform.forward * 0.1f, ForceMode.Impulse);
        }

        // Clear state
        isDocked = false;
        GameObject undockedHalf = dockedHalf;
        dockedHalf = null;

        // Notify station manager
        stationManager.OnUndockingComplete(this, undockedHalf);
    }

    public GameObject GetDockedHalf()
    {
        return dockedHalf;
    }

    public bool IsDocked()
    {
        return isDocked;
    }
}