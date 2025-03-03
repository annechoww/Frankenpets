using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class HindLegsRigging : MonoBehaviour
{
    // target that the hindLegs rig target will rig towards
    public Transform hindLegsTarget;
    public float speed;
    public float range;

    private Vector3 neutralPosition;
    private bool isStanding = false;

    Vector3 oldPosition, newPosition;
    Vector3 oldNormal, newNormal;
    // private bool isMovingNaturally = false;

    void Start(){
        oldNormal = newNormal = transform.up;
    }

    void Update(){
        neutralPosition = hindLegsTarget.position;
    }

    public void stand()
    {
        Debug.Log("standing");
        isStanding = true;
        if (isStanding){
            StartCoroutine(StandOnHindLegs(neutralPosition));
        }
        
    }

    public void turn(float combinedTurn)
    {
        transform.Rotate(0.0f, combinedTurn, 0.0f, Space.Self);
    }

    public void release()
    {
        Debug.Log("stop standing");
        isStanding = false;
    }

    private IEnumerator StandOnHindLegs(Vector3 targetPosition)
    {
        float duration = speed;
        float elapsedTime = 0f;

        oldPosition = targetPosition;
        newPosition = targetPosition + Vector3.forward * range;
        Ray ray =  new Ray(targetPosition, Vector3.down);
        Ray ray2 =  new Ray(newPosition, Vector3.down);
        Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red, duration=5.0f);
        Debug.DrawRay(ray2.origin, ray2.direction * 2f, Color.blue, duration=5.0f);
        
        while (elapsedTime < duration)
        {

            transform.position = Vector3.Lerp(oldPosition, newPosition, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move back to oldPosition (Close)
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(newPosition, neutralPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = neutralPosition;
        

        isStanding = false;
    }
    


}