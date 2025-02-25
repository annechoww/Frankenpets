using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PawRigging : MonoBehaviour
{
    public Transform pawTarget;
    public float verticalMove; 
    public float horizontalMove; 
    public float horizontalOffset;
    public float verticalOffset;
    public float moveSpeed;
    public Rig pawRig;

    Vector3 oldPosition, newPosition;
    private Vector3 pawTargetPosition;

    void Start()
    {
        oldPosition=newPosition=transform.position;
        pawRig.weight = 0f;
    }

    void Update()
    {
        //pawTargetPosition = pawTarget.position + Vector3.down * verticalOffset + transform.right * horizontalOffset;
        pawTargetPosition = pawTarget.position;
    }

    public void liftPaw(){
        pawRig.weight = 1f;
        StartCoroutine(LiftUpPaw(pawTargetPosition));
    }

    private IEnumerator LiftUpPaw(Vector3 targetPosition)
    {
        oldPosition = targetPosition;
        newPosition = targetPosition + transform.right * horizontalMove + Vector3.up * verticalMove;

        // Ray ray =  new Ray(newPosition, Vector3.down);
        // Debug.DrawRay(ray.origin, ray.direction * 2f, Color.red);

        float elapsedTime = 0f;
        float duration = 0.2f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(oldPosition, newPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = newPosition;
        yield return new WaitForSeconds(0.1f);
        
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(newPosition, oldPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = oldPosition;
        pawRig.weight = 0f;
        }

}