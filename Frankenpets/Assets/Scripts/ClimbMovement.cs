using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class ClimbMovement: MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public LayerMask ClimbingWall;

    private bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float wallLookAngle;

    private RaycastHit frontWallHit;
    private bool wallFront;

    public bool checkClimb()
    {
        WallCheck();
        StateMachine();

        if (climbing){
            UnityEngine.Debug.Log("climbing is true");
        } else{
            UnityEngine.Debug.Log("climbing is false;");
        }

        return climbing;
    }

    private void StateMachine()
    {
        UnityEngine.Debug.Log($"StateMachine - wallFront: {wallFront}, wallLookAngle: {wallLookAngle}, maxWallLookAngle: {maxWallLookAngle}");


        UnityEngine.Debug.Log("checking state machine");
        if (wallFront && wallLookAngle < maxWallLookAngle){
            climbing = true;
            
        }
        else{
            UnityEngine.Debug.Log("angle not smaller");
            if (climbing){
                climbing = false;
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereCastRadius);
        Gizmos.DrawLine(transform.position, transform.position + orientation.forward * detectionLength); // Direction
    }

    private void WallCheck()
    {
        UnityEngine.Debug.Log("checking wall");
        

        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, ClimbingWall);
        wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        if (wallFront){
            UnityEngine.Debug.Log("wall if front");
        } else{
            UnityEngine.Debug.Log("wall not front");
        }
        //UnityEngine.Debug.Log("wall look angle: " + wallLookAngle);

        
    }





}