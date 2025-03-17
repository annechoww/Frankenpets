using UnityEngine;
using Unity.Cinemachine;
using System;
using System.Reflection;

public class CameraMovement : MonoBehaviour
{
    [Header("Cameras")]
    public Camera mainCamera;
    public Camera p1Camera;
    public Camera p2Camera;

    [Header("Trackers")]
    public CinemachineCamera mainTracker;
    public CinemachineCamera player1Tracker;
    public CinemachineCamera player2Tracker;

    [Header("Pet")]
    public Transform frontHalf;
    public Transform backHalf;
    
    [Header("Settings")]
    public float minOccluderDistance = 0.55f;
    public float reducedOccluderDistance = 0.30f;
    public float wallDetectionRadius = 1.0f;
    private FixedJoint fixedJoint;
    private bool splitScreen = false;
    private PlayerManager playerManager; 
    private Player P1;
    private Player P2;

    void Start()
    {
        mainCamera.gameObject.SetActive(true);
        p1Camera.gameObject.SetActive(false);
        p2Camera.gameObject.SetActive(false);

        playerManager = FindObjectOfType<PlayerManager>();
        P1 = playerManager.P1;
        P2 = playerManager.P2;
    }

    void Update()
    {
        fixedJoint = frontHalf.GetComponent<FixedJoint>();

        if (fixedJoint != null)
        {
            mainCamera.gameObject.SetActive(true);
            p1Camera.gameObject.SetActive(false);
            p2Camera.gameObject.SetActive(false);
        }
        else
        {
            splitScreen = true;
        }

        // Check if the pet is near a wall
        CheckWallOcclusion();
    }

    void FixedUpdate()
    {
        if (splitScreen)
        {
            mainCamera.gameObject.SetActive(false);
            p1Camera.gameObject.SetActive(true);
            p2Camera.gameObject.SetActive(true);
            splitScreen = false;
        }
    }

    void CheckWallOcclusion()
    {
        if (!splitScreen)
        {
            AdjustCameraOcclusion(frontHalf, mainTracker);
        } else
        {
            if (P1.IsFront)
            {
                AdjustCameraOcclusion(frontHalf, player1Tracker);
                AdjustCameraOcclusion(backHalf, player2Tracker);
            } else 
            {
                AdjustCameraOcclusion(backHalf, player1Tracker);
                AdjustCameraOcclusion(frontHalf, player2Tracker);
            }
            
        }
    }

    void AdjustCameraOcclusion(Transform petHalf, CinemachineCamera tracker)
    {
        // Detect nearby colliders
        Collider[] hitColliders = Physics.OverlapSphere(petHalf.position, wallDetectionRadius);

        bool isNearWall = false;

        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Wall"))
            {
                isNearWall = true;
                break;
            }
        }
        float newDistance = isNearWall ? reducedOccluderDistance : minOccluderDistance;
        
        var deoccluder = tracker.GetComponent<CinemachineDeoccluder>();
        if (deoccluder != null)
        {
            deoccluder.MinimumDistanceFromTarget = newDistance;
        }
        else{
            Debug.LogError("Camera Deoccluder not found.");
        }
    }
}
