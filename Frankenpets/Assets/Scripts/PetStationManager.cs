using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class PetStationManager : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public DockingPoint[] dockingPoints;
    
    [Header("Configuration")]
    public float dockingRadius = 0.5f; // How close the magnet needs to be to dock

    private Dictionary<string, DockingPoint> dockedHalves = new Dictionary<string, DockingPoint>();

    private LivingRoomTextManager livingRoomText;

    void Awake()
    {
        livingRoomText = GameObject.Find("TextManager").GetComponent<LivingRoomTextManager>();
    }

    void Start()
    {
        // Validate setup
        if (playerManager == null)
        {
            Debug.LogError("PetStationManager missing PlayerManager reference!");
            return;
        }

        if (dockingPoints == null || dockingPoints.Length != 4)
        {
            Debug.LogError("PetStationManager needs exactly 4 docking points!");
            return;
        }
    }

    public bool ValidateDockingAttempt(DockingPoint dockingPoint, GameObject petHalf)
    {
        // Check if this half is already docked somewhere
        if (dockedHalves.ContainsKey(petHalf.name)) {
            Debug.Log($"Pet half {petHalf.name} is already docked!");
            return false;
        }

        return true;
    }

    public bool RequestDocking(DockingPoint dockingPoint, GameObject petHalf)
    {
        // Final validation before docking
        if (!ValidateDockingAttempt(dockingPoint, petHalf))
            return false;

        // Decide if we're dealing with a front half or a back half.
        bool isFrontHalf = petHalf.name == "catFront" || petHalf.name == "dogFront";
        bool isBackHalf  = petHalf.name == "catBack"  || petHalf.name == "dogBack";

        if (isFrontHalf)
        {
            // Opposite front: dogFront ↔ catFront
            string oppositeFrontName = (petHalf.name == "catFront") ? "dogFront" : "catFront";

            // If the opposite front is docked, swap it out
            if (dockedHalves.ContainsKey(oppositeFrontName))
            {
                DockingPoint oldDockPoint = dockedHalves[oppositeFrontName];
                StartCoroutine(SwapHalves(oldDockPoint));
            }
        }
        else if (isBackHalf)
        {
            // Opposite back: dogBack ↔ catBack
            string oppositeBackName = (petHalf.name == "catBack") ? "dogBack" : "catBack";

            // If the opposite back is docked, swap it out
            if (dockedHalves.ContainsKey(oppositeBackName))
            {
                DockingPoint oldDockPoint = dockedHalves[oppositeBackName];
                StartCoroutine(SwapHalves(oldDockPoint));
            }
        }

        // Add the new half to the dictionary
        dockedHalves[petHalf.name] = dockingPoint;
        return true;
    }   

    private IEnumerator SwapHalves(DockingPoint oldDockPoint)
    {
        // Wait 1 frame so the new half can finish docking
        yield return null;

        // Undock the old half
        oldDockPoint.UndockCurrentHalf();
        // Note: we do NOT remove it from the dictionary here.
        // OnUndockingComplete will handle dictionary removal.
    }
    public void OnDockingComplete(DockingPoint dockingPoint, GameObject petHalf)
    {
        Debug.Log($"Docking complete: {petHalf.name} at {dockingPoint.name}");

        // Notify PlayerManager to update control
        TransferControlToCounterpart(petHalf);
    }

    public void OnUndockingComplete(DockingPoint dockingPoint, GameObject petHalf)
    {
        // Remove from tracked docked halves
        if (dockedHalves.ContainsKey(petHalf.name))
            dockedHalves.Remove(petHalf.name);
    }

    private string GetCounterpartName(string halfName)
    {
        switch (halfName)
        {
            case "catFront": return "dogFront";
            case "dogFront": return "catFront";
            case "catBack": return "dogBack";
            case "dogBack": return "catBack";
            default: return "";
        }
    }

    private void TransferControlToCounterpart(GameObject dockedHalf)
    {
        string counterpartName = GetCounterpartName(dockedHalf.name);
        
        // Find the counterpart GameObject in the scene
        Transform playerParent = dockedHalf.transform.parent;
        Transform petParent = playerParent.parent;
        
        GameObject counterpart = null;
        foreach (Transform player in petParent)
        {
            foreach (Transform half in player)
            {
                if (half.gameObject.name == counterpartName)
                {
                    counterpart = half.gameObject;
                    break;
                }
            }
            if (counterpart != null) break;
        }

        if (counterpart != null)
        {
            // Update player control through PlayerManager
            playerManager.TransferControl(dockedHalf, counterpart);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("cat front") || other.CompareTag("cat back") || other.CompareTag("dog front") || other.CompareTag("dog back"))
        {
            livingRoomText.advanceLivingRoomStage();
        }
    }
}