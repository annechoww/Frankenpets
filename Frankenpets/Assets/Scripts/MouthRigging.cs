using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class MouthRigging : MonoBehaviour
{
    public Transform mouthTarget;
    public float moveDistance; // Distance to move down
    public float horizontalOffset; // Distance to move down
    public float verticalOffset; // Distance to move down
    public float moveSpeed; // Speed of movement

    Vector3 oldPosition, newPosition;
    Vector3 oldNormal, newNormal;

    private Vector3 mouthTargetPosition;

    void Start()
    {
        oldPosition=newPosition=transform.position;
        oldNormal=newNormal=transform.up;
    }

    void Update()
    {
        //mouthTargetPosition = mouthTarget.position;
        mouthTargetPosition = mouthTarget.position + transform.right * horizontalOffset + Vector3.down * verticalOffset;
        
    }

    // public void grab(){

    //     transform.position = currentPosition;
    //     transform.up = currentNormal;
    //     Ray ray =  new Ray(targetBody.position, Vector3.down);

    //     if (Vector3.Distance(newPosition, hit.point) > stepDistance && !otherFoot.isMoving() && lerp >= 1)
    //         {
    //             lerp = 0;
    //             int direction = body.InverseTransformPoint(hit.point).z > body.InverseTransformPoint(newPosition).z ? 1 : -1;
    //             newPosition = hit.point + (body.forward * stepLength * direction) + footOffset;
    //             newNormal = hit.normal;
    //         }
        
    //     // targetPosition = originalPosition + Vector3.down * moveDistance;
    //     // transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * moveSpeed);
        
    // }

    // public void release(){
    //     transform.position = Vector3.Lerp(transform.position, originalPosition, Time.deltaTime * moveSpeed);
    // }


    public void openMouth(){
        StartCoroutine(OpenAndClose(mouthTargetPosition));
    }

    private IEnumerator OpenAndClose(Vector3 targetPosition)
    {
        oldPosition = targetPosition;
        newPosition = targetPosition + Vector3.down * moveDistance;

        float elapsedTime = 0f;
        float duration = 0.2f; // Time to fully open

        // Move to newPosition (Open)
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(oldPosition, newPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;

        yield return new WaitForSeconds(0.1f); // Wait 1 second before closing

        elapsedTime = 0f;

        // Move back to oldPosition (Close)
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(newPosition, oldPosition, elapsedTime / duration);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = oldPosition;
        }

}