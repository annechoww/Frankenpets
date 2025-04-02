using UnityEngine;
using UnityEngine.Animations.Rigging;
using System.Collections;

public class PawRigging : MonoBehaviour
{

    public float verticalMove; 
    public float horizontalMove; 
    public float moveSpeed;
    public Rig pawRig;

    private Vector3 oldPosition, newPosition;

    void Start()
    {
        oldPosition=newPosition=transform.localPosition;
        pawRig.weight = 0f;
    }

    public void liftPaw(){
        pawRig.weight = 1f;
        StartCoroutine(LiftUpPaw());
    }

    private IEnumerator LiftUpPaw()
    {
        float elapsedTime = 0f;
        float duration = 0.2f;

        newPosition = transform.localPosition + Vector3.right * horizontalMove + Vector3.up * verticalMove;

        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        yield return new WaitForSeconds(0.1f); // Wait 1 second before closing

        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, oldPosition, Time.deltaTime * moveSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = oldPosition;
        pawRig.weight = 0f;
        }

}